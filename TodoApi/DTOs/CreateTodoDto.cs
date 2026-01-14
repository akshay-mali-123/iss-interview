namespace TodoApi.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new TODO item
    /// </summary>
    public class CreateTodoDto
    {
        /// <summary>
        /// Gets or sets the title of the TODO item
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the description of the TODO item
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets whether the TODO item is completed
        /// </summary>
        public bool IsCompleted { get; set; } = false;
    }
}
