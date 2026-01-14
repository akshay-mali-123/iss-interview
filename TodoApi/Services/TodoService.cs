using TodoApi.DTOs;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Services
{
    /// <summary>
    /// Service implementation for TODO business logic
    /// </summary>
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ILogger<TodoService> _logger;

        /// <summary>
        /// Initializes a new instance of the TodoService
        /// </summary>
        /// <param name="todoRepository">The TODO repository</param>
        /// <param name="logger">Logger instance</param>
        public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto)
        {
            _logger.LogInformation("Creating new todo with title: {Title}", createTodoDto.Title);

            var todo = new Todo
            {
                Title = createTodoDto.Title,
                Description = createTodoDto.Description,
                IsCompleted = createTodoDto.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            var createdTodo = await _todoRepository.AddAsync(todo);
            
            _logger.LogInformation("Successfully created todo with id: {Id}", createdTodo.Id);
            
            return MapToDto(createdTodo);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTodoAsync(int id)
        {
            _logger.LogInformation("Deleting todo with id: {Id}", id);

            var exists = await _todoRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Todo with id {Id} not found for deletion", id);
                return false;
            }

            var result = await _todoRepository.DeleteAsync(id);
            
            if (result)
            {
                _logger.LogInformation("Successfully deleted todo with id: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete todo with id: {Id}", id);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
        {
            _logger.LogInformation("Retrieving all todos");

            var todos = await _todoRepository.GetAllAsync();
            return todos.Select(MapToDto);
        }

        /// <inheritdoc />
        public async Task<TodoDto?> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving todo with id: {Id}", id);

            var todo = await _todoRepository.GetByIdAsync(id);
            return todo != null ? MapToDto(todo) : null;
        }

        /// <inheritdoc />
        public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto)
        {
            _logger.LogInformation("Updating todo with id: {Id}", id);

            var existingTodo = await _todoRepository.GetByIdAsync(id);
            if (existingTodo == null)
            {
                _logger.LogWarning("Todo with id {Id} not found for update", id);
                return null;
            }

            existingTodo.Title = updateTodoDto.Title;
            existingTodo.Description = updateTodoDto.Description;
            existingTodo.IsCompleted = updateTodoDto.IsCompleted;

            var updatedTodo = await _todoRepository.UpdateAsync(existingTodo);
            
            _logger.LogInformation("Successfully updated todo with id: {Id}", id);
            
            return MapToDto(updatedTodo);
        }

        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            };
        }
    }
}
