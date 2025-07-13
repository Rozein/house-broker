using FluentValidation;
using HouseBroker.Domain.Enums;

namespace HouseBroker.Application.Features.Auth.Command.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters");

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

        // First name validation
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MinimumLength(2)
            .WithMessage("First name must be at least 2 characters long")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        // Last name validation
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MinimumLength(2)
            .WithMessage("Last name must be at least 2 characters long")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$")
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        // Phone number validation
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number must be a valid international format (e.g., +1234567890)")
            .MinimumLength(10)
            .WithMessage("Phone number must be at least 10 digits")
            .MaximumLength(15)
            .WithMessage("Phone number cannot exceed 15 digits");

        // Role validation
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be a valid role type (Admin, Broker, or HouseSeeker)");

        // Conditional validation for Broker-specific fields
        When(x => x.Role == RoleEnum.Broker, () =>
        {
            RuleFor(x => x.LicenseNumber)
                .NotEmpty()
                .WithMessage("License number is required for broker registration")
                .MinimumLength(5)
                .WithMessage("License number must be at least 5 characters long")
                .MaximumLength(50)
                .WithMessage("License number cannot exceed 50 characters")
                .Matches(@"^[A-Z0-9\-]+$")
                .WithMessage("License number can only contain uppercase letters, numbers, and hyphens");

            RuleFor(x => x.AgencyId)
                .NotEmpty()
                .WithMessage("Agency ID is required for broker registration")
                .NotEqual(Guid.Empty)
                .WithMessage("Agency ID cannot be empty");
        });

        // Ensure non-broker roles don't have broker-specific fields
        When(x => x.Role != RoleEnum.Broker, () =>
        {
            RuleFor(x => x.LicenseNumber)
                .Empty()
                .WithMessage("License number should only be provided for broker registration");

            RuleFor(x => x.AgencyId)
                .Empty()
                .WithMessage("Agency ID should only be provided for broker registration");
        });
    }
}