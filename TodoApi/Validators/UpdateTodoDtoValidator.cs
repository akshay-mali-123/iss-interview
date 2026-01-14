using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validators
{
    /// <summary>
    /// Validator for UpdateTodoDto
    /// </summary>
    public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
    {
        /// <summary>
        /// Initializes a new instance of the UpdateTodoDtoValidator
        /// </summary>
        public UpdateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
