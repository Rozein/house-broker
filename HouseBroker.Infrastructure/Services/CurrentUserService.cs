using System.Security.Claims;
using HouseBroker.Application.CurrentUserService;
using Microsoft.AspNetCore.Http;

namespace HouseBroker.Infrastructure.Services;
/// <summary>
/// Provides information about the currently authenticated user by accessing the HTTP context.
/// </summary>
/// <param name="httpContextAccessor">Used to access the current HttpContext.</param>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim?.Value, out var guid)
                ? guid
                : Guid.Empty;
        }
    }    
    public string? UserRole =>  httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}