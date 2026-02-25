using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Application.Features.Products.Commands
{
    public class CreateProductCommandValidator:AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {

            // Name is required and max 100 characters
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty().WithMessage("{PropertyName} is required")
                .Length(2, 100).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters long.");


            // Rate must be greater than 0
            RuleFor(x => x.Rate)
                        .NotNull()
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than zero")
        .LessThan(500).WithMessage("{PropertyName} must be less than 500");
        }
    }
}
