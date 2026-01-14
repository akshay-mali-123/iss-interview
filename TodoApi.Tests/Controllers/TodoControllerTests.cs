using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Controllers;
using TodoApi.DTOs;
using TodoApi.Interfaces;
using Xunit;

namespace TodoApi.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly Mock<IValidator<CreateTodoDto>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateTodoDto>> _mockUpdateValidator;
        private readonly Mock<ILogger<TodoController>> _mockLogger;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _mockCreateValidator = new Mock<IValidator<CreateTodoDto>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateTodoDto>>();
            _mockLogger = new Mock<ILogger<TodoController>>();

            _controller = new TodoController(
                _mockTodoService.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllTodos_ReturnsOkWithTodos()
        {
            // Arrange
            var todos = new List<TodoDto>
            {
                new() { Id = 1, Title = "Test Todo 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Title = "Test Todo 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
            };
            
            _mockTodoService.Setup(s => s.GetAllTodosAsync()).ReturnsAsync(todos);

            // Act
            var result = await _controller.GetAllTodos();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(todos);
        }

        [Fact]
        public async Task GetTodo_WithValidId_ReturnsOkWithTodo()
        {
            // Arrange
            const int todoId = 1;
            var todo = new TodoDto { Id = todoId, Title = "Test Todo", IsCompleted = false, CreatedAt = DateTime.UtcNow };
            
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(todoId)).ReturnsAsync(todo);

            // Act
            var result = await _controller.GetTodo(todoId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(todo);
        }

        [Fact]
        public async Task GetTodo_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            const int todoId = 999;
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(todoId)).ReturnsAsync((TodoDto?)null);

            // Act
            var result = await _controller.GetTodo(todoId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateTodo_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateTodoDto { Title = "New Todo", Description = "Description" };
            var createdTodo = new TodoDto { Id = 1, Title = "New Todo", Description = "Description", IsCompleted = false, CreatedAt = DateTime.UtcNow };

            _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());
            _mockTodoService.Setup(s => s.CreateTodoAsync(createDto)).ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.CreateTodo(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.Value.Should().BeEquivalentTo(createdTodo);
        }

        [Fact]
        public async Task UpdateTodo_WithValidData_ReturnsOkWithUpdatedTodo()
        {
            // Arrange
            const int todoId = 1;
            var updateDto = new UpdateTodoDto { Title = "Updated Todo", Description = "Updated Description", IsCompleted = true };
            var updatedTodo = new TodoDto { Id = todoId, Title = "Updated Todo", Description = "Updated Description", IsCompleted = true, CreatedAt = DateTime.UtcNow };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());
            _mockTodoService.Setup(s => s.UpdateTodoAsync(todoId, updateDto)).ReturnsAsync(updatedTodo);

            // Act
            var result = await _controller.UpdateTodo(todoId, updateDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(updatedTodo);
        }

        [Fact]
        public async Task DeleteTodo_WithValidId_ReturnsOkWithMessage()
        {
            // Arrange
            const int todoId = 1;
            _mockTodoService.Setup(s => s.DeleteTodoAsync(todoId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTodo(todoId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }
    }
}
