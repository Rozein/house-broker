using HouseBroker.API.Attributes;
using HouseBroker.Application.Features.Auth.Command.Login;
using HouseBroker.Application.Features.Auth.Command.RegisterUser;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.API.Controllers;

public class IdentityController: APIControllerBase
{
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="command">Login credentials containing email and password</param>
    /// <returns>Authentication response with JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<Response<AuthResponseDto>> Authenticate([FromBody] LoginUserCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Registers a new user with the provided information
    /// </summary>
    /// <param name="command">Registration details including email, password, name, phone number, role, license number, and agency ID</param>
    /// <returns>Authentication response containing JWT token, token expiration, and user information</returns>
    [HttpPost("register")]
    public async Task<Response<AuthResponseDto>> RegisterUser([FromBody] RegisterUserCommand command)
    {
        return await Mediator.Send(command);
    }
}