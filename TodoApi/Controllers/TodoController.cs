using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Interfaces;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Controller for managing TODO items
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IValidator<CreateTodoDto> _createValidator;
        private readonly IValidator<UpdateTodoDto> _updateValidator;
        private readonly ILogger<TodoController> _logger;

        /// <summary>
        /// Initializes a new instance of the TodoController
        /// </summary>
        /// <param name="todoService">The TODO service</param>
        /// <param name="createValidator">Validator for creating TODOs</param>
        /// <param name="updateValidator">Validator for updating TODOs</param>
        /// <param name="logger">Logger instance</param>
        public TodoController(
            ITodoService todoService,
            IValidator<CreateTodoDto> createValidator,
            IValidator<UpdateTodoDto> updateValidator,
            ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        /// <summary>
        /// Get all TODO items
        /// </summary>
        /// <returns>List of all TODO items</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetAllTodos()
        {
            _logger.LogInformation("Getting all todos");
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        /// <summary>
        /// Get a specific TODO item by ID
        /// </summary>
        /// <param name="id">The ID of the TODO item</param>
        /// <returns>The TODO item if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDto>> GetTodo(int id)
        {
            _logger.LogInformation("Getting todo with id: {Id}", id);
            
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound($"Todo with id {id} not found");
            }

            return Ok(todo);
        }

        /// <summary>
        /// Create a new TODO item
        /// </summary>
        /// <param name="createTodoDto">The TODO item to create</param>
        /// <returns>The created TODO item</returns>
        [HttpPost]
        public async Task<ActionResult<TodoDto>> CreateTodo([FromBody] CreateTodoDto createTodoDto)
        {
            _logger.LogInformation("Creating new todo");

            var validationResult = await _createValidator.ValidateAsync(createTodoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var todo = await _todoService.CreateTodoAsync(createTodoDto);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }

        /// <summary>
        /// Update an existing TODO item
        /// </summary>
        /// <param name="id">The ID of the TODO item to update</param>
        /// <param name="updateTodoDto">The updated TODO item data</param>
        /// <returns>The updated TODO item</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoDto>> UpdateTodo(int id, [FromBody] UpdateTodoDto updateTodoDto)
        {
            _logger.LogInformation("Updating todo with id: {Id}", id);

            var validationResult = await _updateValidator.ValidateAsync(updateTodoDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var todo = await _todoService.UpdateTodoAsync(id, updateTodoDto);
            if (todo == null)
            {
                return NotFound($"Todo with id {id} not found");
            }

            return Ok(todo);
        }

        /// <summary>
        /// Delete a TODO item
        /// </summary>
        /// <param name="id">The ID of the TODO item to delete</param>
        /// <returns>Success message if deleted</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            _logger.LogInformation("Deleting todo with id: {Id}", id);

            var result = await _todoService.DeleteTodoAsync(id);
            if (!result)
            {
                return NotFound($"Todo with id {id} not found");
            }

            return Ok(new { message = "Todo deleted successfully" });
        }
    }
}
