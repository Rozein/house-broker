using FluentValidation;

namespace HouseBroker.Application.Features.Property.Command.CreateProperty;

public class CreatePropertyCommandValidator: AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.PropertyType)
            .IsInEnum().WithMessage("Invalid property type.");

        // Conditional validation:
        When(x => !x.LocationId.HasValue || x.LocationId == Guid.Empty, () =>
        {
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("CityId is required when LocationId is not provided.");

            RuleFor(x => x.Area)
                .NotEmpty().WithMessage("Area is required when LocationId is not provided.");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("PostalCode is required when LocationId is not provided.");
        });
        
        RuleFor(x => x.Feature)
            .NotEmpty()
            .WithMessage("Property feature in required.");

    }
}