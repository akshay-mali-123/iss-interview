namespace TodoApi.DTOs
{
    /// <summary>
    /// Data transfer object for TODO item responses
    /// </summary>
    public class TodoDto
    {
        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public required string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets the completion status
        /// </summary>
        public bool IsCompleted { get; set; }
        
        /// <summary>
        /// Gets or sets the creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
