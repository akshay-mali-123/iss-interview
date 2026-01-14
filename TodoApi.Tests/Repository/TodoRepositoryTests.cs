using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Models;
using TodoApi.Repository;
using Xunit;

namespace TodoApi.Tests.Repository
{
    public class TodoRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TodoRepository _repository;
        private readonly string _connectionString;

        public TodoRepositoryTests()
        {
            _connectionString = "Data Source=test_" + Guid.NewGuid().ToString() + ".db";
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();

            // Create table
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                )";
            command.ExecuteNonQuery();

            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection", _connectionString}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var mockLogger = new Mock<ILogger<TodoRepository>>();
            _repository = new TodoRepository(configuration, mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_WithValidTodo_ReturnsTodoWithId()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.AddAsync(todo);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Title.Should().Be("Test Todo");
            result.Description.Should().Be("Test Description");
            result.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Existing Todo",
                Description = "Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var added = await _repository.AddAsync(todo);

            // Act
            var result = await _repository.GetByIdAsync(added.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(added.Id);
            result.Title.Should().Be("Existing Todo");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTodos()
        {
            // Arrange
            await _repository.AddAsync(new Todo { Title = "Todo 1", CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new Todo { Title = "Todo 2", CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new Todo { Title = "Todo 3", CreatedAt = DateTime.UtcNow });

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdateAsync_WithValidTodo_UpdatesTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Original Title",
                Description = "Original Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var added = await _repository.AddAsync(todo);

            // Act
            added.Title = "Updated Title";
            added.Description = "Updated Description";
            added.IsCompleted = true;
            var result = await _repository.UpdateAsync(added);

            // Assert
            var updated = await _repository.GetByIdAsync(added.Id);
            updated.Should().NotBeNull();
            updated!.Title.Should().Be("Updated Title");
            updated.Description.Should().Be("Updated Description");
            updated.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WithExistingId_DeletesTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "To Delete",
                CreatedAt = DateTime.UtcNow
            };
            var added = await _repository.AddAsync(todo);

            // Act
            var result = await _repository.DeleteAsync(added.Id);

            // Assert
            result.Should().BeTrue();
            var deleted = await _repository.GetByIdAsync(added.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_WithExistingId_ReturnsTrue()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Existing",
                CreatedAt = DateTime.UtcNow
            };
            var added = await _repository.AddAsync(todo);

            // Act
            var result = await _repository.ExistsAsync(added.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetCompletedTodosAsync_ReturnsOnlyCompletedTodos()
        {
            // Arrange
            await _repository.AddAsync(new Todo { Title = "Completed 1", IsCompleted = true, CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new Todo { Title = "Pending", IsCompleted = false, CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new Todo { Title = "Completed 2", IsCompleted = true, CreatedAt = DateTime.UtcNow });

            // Act
            var result = await _repository.GetCompletedTodosAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.IsCompleted);
        }

        [Fact]
        public async Task GetPendingTodosAsync_ReturnsOnlyPendingTodos()
        {
            // Arrange
            await _repository.AddAsync(new Todo { Title = "Pending 1", IsCompleted = false, CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new Todo { Title = "Completed", IsCompleted = true, CreatedAt = DateTime.UtcNow });
            await _repository.AddAsync(new Todo { Title = "Pending 2", IsCompleted = false, CreatedAt = DateTime.UtcNow });

            // Act
            var result = await _repository.GetPendingTodosAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => !t.IsCompleted);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            
            // Clean up the database file with retry logic
            var dbFile = _connectionString.Replace("Data Source=", "");
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                
                if (File.Exists(dbFile))
                {
                    File.Delete(dbFile);
                }
            }
            catch
            {
                // Ignore cleanup errors - file will be cleaned up by test runner
            }
        }
    }
}
