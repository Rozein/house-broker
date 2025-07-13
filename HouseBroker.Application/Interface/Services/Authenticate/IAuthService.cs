using HouseBroker.Application.Interface.DIRegistration;

namespace HouseBroker.Application.Interface.Services.Authenticate;

public interface IAuthService : IScopedDependency
{
    Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<Response> LogoutAsync(string userId);
}