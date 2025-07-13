using FluentValidation;
using HouseBroker.Application.Features.Property.Command.DeleteProperty;

namespace HouseBroker.Application.Property.Command.DeleteProperty;

public class DeletePropertyCommandValidator : AbstractValidator<DeletePropertyCommand>
{
    public DeletePropertyCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}