using Microsoft.Data.Sqlite;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Repository
{
    /// <summary>
    /// Repository implementation for TODO data access
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TodoRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the TodoRepository
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        /// <param name="logger">Logger instance</param>
        public TodoRepository(IConfiguration configuration, ILogger<TodoRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=todos.db";
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Todo> AddAsync(Todo entity)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                    VALUES (@title, @description, @isCompleted, @createdAt);
                    SELECT last_insert_rowid();";

                command.Parameters.AddWithValue("@title", entity.Title);
                command.Parameters.AddWithValue("@description", entity.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCompleted", entity.IsCompleted ? 1 : 0);
                command.Parameters.AddWithValue("@createdAt", entity.CreatedAt.ToString("o"));

                var id = Convert.ToInt32(await command.ExecuteScalarAsync());
                entity.Id = id;
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo with title: {Title}", entity.Title);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo with id: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(1) FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if todo exists with id: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            try
            {
                var todos = new List<Todo>();
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos ORDER BY CreatedAt DESC";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    todos.Add(MapReaderToTodo(reader));
                }

                return todos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all todos");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Todo?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return MapReaderToTodo(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving todo with id: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetCompletedTodosAsync()
        {
            try
            {
                var todos = new List<Todo>();
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE IsCompleted = 1 ORDER BY CreatedAt DESC";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    todos.Add(MapReaderToTodo(reader));
                }

                return todos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed todos");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetPendingTodosAsync()
        {
            try
            {
                var todos = new List<Todo>();
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE IsCompleted = 0 ORDER BY CreatedAt DESC";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    todos.Add(MapReaderToTodo(reader));
                }

                return todos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending todos");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Todo> UpdateAsync(Todo entity)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Todos
                    SET Title = @title, Description = @description, IsCompleted = @isCompleted
                    WHERE Id = @id";

                command.Parameters.AddWithValue("@title", entity.Title);
                command.Parameters.AddWithValue("@description", entity.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCompleted", entity.IsCompleted ? 1 : 0);
                command.Parameters.AddWithValue("@id", entity.Id);

                await command.ExecuteNonQueryAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo with id: {Id}", entity.Id);
                throw;
            }
        }

        private static Todo MapReaderToTodo(SqliteDataReader reader)
        {
            var descriptionOrdinal = reader.GetOrdinal("Description");
            return new Todo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                IsCompleted = reader.GetInt32(reader.GetOrdinal("IsCompleted")) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
            };
        }
    }
}
