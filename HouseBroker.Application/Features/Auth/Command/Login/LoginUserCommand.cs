using HouseBroker.Application.Interface.Messaging;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Application.Interface.Services.Authenticate;
using Microsoft.Extensions.Logging;

namespace HouseBroker.Application.Features.Auth.Command.Login;

public class LoginUserCommand : ICommand<AuthResponseDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(IAuthService authService, ILogger<LoginUserCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<Response<AuthResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt started for user: {Email}", request.Email);

        var loginDto = new LoginDto
        {
            Email = request.Email,
            Password = request.Password
        };
        var login = await _authService.LoginAsync(loginDto);
        if (!login.IsSuccess)
        {
            _logger.LogWarning("Login failed for user: {Email}. Reason: {Error}", request.Email, login.Error?.ToString());
        }
        else
        {
            _logger.LogInformation("Login successful for user: {Email}", request.Email);
        }
        return login;
    }
}