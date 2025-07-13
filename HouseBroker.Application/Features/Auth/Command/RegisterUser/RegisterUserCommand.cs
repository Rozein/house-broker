using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Application.Interface.Services.Authenticate;
using HouseBroker.Domain.Enums;

namespace HouseBroker.Application.Features.Auth.Command.RegisterUser;

public class RegisterUserCommand : ICommand<AuthResponseDto>
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PhoneNumber { get; init; }
    public RoleEnum Role { get; init; }
    
    // Broker-specific fields
    public string? LicenseNumber { get; init; }
    public Guid? AgencyId { get; init; }
}

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Response<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(new RegisterDto
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            LicenseNumber = request.LicenseNumber,
            AgencyId = request.AgencyId
        });

    }
}