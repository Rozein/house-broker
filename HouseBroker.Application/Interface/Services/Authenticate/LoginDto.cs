using HouseBroker.Domain.Enums;

namespace HouseBroker.Application.Interface.Services;

public class LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
public class RegisterDto
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PhoneNumber { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Address { get; init; }

    public RoleEnum Role { get; init; }
    
    // Broker-specific fields
    public string? LicenseNumber { get; init; }
    public Guid? AgencyId { get; init; }
}

public class AuthResponseDto
{
    public string Token { get; init; }
    public DateTime? TokenExpiration { get; init; }
    public UserDto User { get; init; }
}

public class UserDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public List<string> Roles { get; init; }
}