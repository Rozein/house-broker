using FluentValidation;

namespace HouseBroker.Application.Property.Command.UpdateProperty;

public class UpdatePropertyCommandValidator :AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyCommandValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.PropertyType)
            .IsInEnum().WithMessage("Invalid property type.");

        RuleFor(x => x.LocationId)
            .NotEmpty().When(x => x.CityId == null && string.IsNullOrEmpty(x.Area) && string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Either LocationId or full location details must be provided.");

        RuleFor(x => x.CityId)
            .NotEmpty().When(x => !x.LocationId.HasValue).WithMessage("CityId is required if LocationId is not provided.");

        RuleFor(x => x.Area)
            .NotEmpty().When(x => !x.LocationId.HasValue).WithMessage("Area is required if LocationId is not provided.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().When(x => !x.LocationId.HasValue).WithMessage("PostalCode is required if LocationId is not provided.");

        RuleFor(x => x.Feature)
            .NotNull().WithMessage("PropertyFeatures list is required.");
    }
}