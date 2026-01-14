using TodoApi.Models;

namespace TodoApi.Interfaces
{
    /// <summary>
    /// Repository interface for TODO-specific data operations
    /// </summary>
    public interface ITodoRepository : IRepository<Todo>
    {
        /// <summary>
        /// Gets all completed TODO items
        /// </summary>
        /// <returns>Collection of completed TODO items</returns>
        Task<IEnumerable<Todo>> GetCompletedTodosAsync();
        
        /// <summary>
        /// Gets all pending TODO items
        /// </summary>
        /// <returns>Collection of pending TODO items</returns>
        Task<IEnumerable<Todo>> GetPendingTodosAsync();
    }
}
