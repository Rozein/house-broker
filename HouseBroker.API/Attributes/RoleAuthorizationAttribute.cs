using HouseBroker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HouseBroker.API.Attributes;

// Custom authorization attribute to restrict access based on user roles.
// Usage: Apply on controller or action method to specify required roles.
//
// Example:
// [RoleAuthorization(RoleEnum.Admin, RoleEnum.Manager)]
// This allows access only to users having Admin or Manager roles.
//
// Inherits from AuthorizeAttribute to leverage built-in ASP.NET Core authorization mechanisms.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RoleAuthorizationAttribute : AuthorizeAttribute
{
    // Constructor accepting multiple roles as params array
    // Throws an exception if no roles are specified
    // Converts RoleEnum values to comma-separated string assigned to the Roles property
    public RoleAuthorizationAttribute(params RoleEnum[] roles)
    {
        if (roles == null || roles.Length == 0)
            throw new ArgumentException("At least one role must be specified", nameof(roles));

        // Convert enum roles to comma-separated string
        Roles = string.Join(",", roles.Select(r => r.ToString()));
    }

    // Constructor accepting a single role
    // Assigns the role as string directly to the Roles property
    public RoleAuthorizationAttribute(RoleEnum role)
    {
        Roles = role.ToString();
    }
}