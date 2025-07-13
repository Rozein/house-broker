using System.Security.Claims;
using HouseBroker.Application.Interface.DIRegistration;
using HouseBroker.Domain.Entities;

namespace HouseBroker.Application;

public interface IJwtTokenService :IScopedDependency
{
    Task<string> GenerateTokenAsync(User user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}