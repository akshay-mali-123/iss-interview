using FluentValidation;
using TodoApi.DTOs;

namespace TodoApi.Validators
{
    /// <summary>
    /// Validator for CreateTodoDto
    /// </summary>
    public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
    {
        /// <summary>
        /// Initializes a new instance of the CreateTodoDtoValidator
        /// </summary>
        public CreateTodoDtoValidator()
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
