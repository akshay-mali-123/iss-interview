using TodoApi.DTOs;

namespace TodoApi.Interfaces
{
    /// <summary>
    /// Service interface for TODO business logic operations
    /// </summary>
    public interface ITodoService
    {
        /// <summary>
        /// Creates a new TODO item
        /// </summary>
        /// <param name="createTodoDto">The TODO creation data</param>
        /// <returns>The created TODO item</returns>
        Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto);
        
        /// <summary>
        /// Gets a TODO item by its identifier
        /// </summary>
        /// <param name="id">The TODO identifier</param>
        /// <returns>The TODO item if found, otherwise null</returns>
        Task<TodoDto?> GetTodoByIdAsync(int id);
        
        /// <summary>
        /// Gets all TODO items
        /// </summary>
        /// <returns>Collection of all TODO items</returns>
        Task<IEnumerable<TodoDto>> GetAllTodosAsync();
        
        /// <summary>
        /// Updates an existing TODO item
        /// </summary>
        /// <param name="id">The TODO identifier</param>
        /// <param name="updateTodoDto">The update data</param>
        /// <returns>The updated TODO item if found, otherwise null</returns>
        Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateTodoDto);
        
        /// <summary>
        /// Deletes a TODO item
        /// </summary>
        /// <param name="id">The TODO identifier</param>
        /// <returns>True if deleted, otherwise false</returns>
        Task<bool> DeleteTodoAsync(int id);
    }
}
