namespace TodoApi.DTOs
{
    /// <summary>
    /// Data transfer object for updating an existing TODO item
    /// </summary>
    public class UpdateTodoDto
    {
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets the completion status
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
