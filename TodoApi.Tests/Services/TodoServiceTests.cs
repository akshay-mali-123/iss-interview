using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.DTOs;
using TodoApi.Interfaces;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests.Services
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepository;
        private readonly Mock<ILogger<TodoService>> _mockLogger;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _mockLogger = new Mock<ILogger<TodoService>>();
            _service = new TodoService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTodoAsync_WithValidData_ReturnsCreatedTodo()
        {
            // Arrange
            var createDto = new CreateTodoDto { Title = "Test Todo", Description = "Test Description" };
            var createdTodo = new Todo { Id = 1, Title = "Test Todo", Description = "Test Description", CreatedAt = DateTime.UtcNow };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Todo>())).ReturnsAsync(createdTodo);

            // Act
            var result = await _service.CreateTodoAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be(createDto.Title);
            result.Description.Should().Be(createDto.Description);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithExistingId_ReturnsTodo()
        {
            // Arrange
            const int todoId = 1;
            var todo = new Todo { Id = todoId, Title = "Test Todo", CreatedAt = DateTime.UtcNow };

            _mockRepository.Setup(r => r.GetByIdAsync(todoId)).ReturnsAsync(todo);

            // Act
            var result = await _service.GetTodoByIdAsync(todoId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(todoId);
            result.Title.Should().Be(todo.Title);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            const int todoId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(todoId)).ReturnsAsync((Todo?)null);

            // Act
            var result = await _service.GetTodoByIdAsync(todoId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTodoAsync_WithExistingTodo_ReturnsTrue()
        {
            // Arrange
            const int todoId = 1;
            _mockRepository.Setup(r => r.ExistsAsync(todoId)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(todoId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteTodoAsync(todoId);

            // Assert
            result.Should().BeTrue();
        }
    }
}
