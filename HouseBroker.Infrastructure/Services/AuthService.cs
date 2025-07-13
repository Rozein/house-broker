using System.Net;
using HouseBroker.Application;
using HouseBroker.Application.CommonDto;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Application.Interface.Services.Authenticate;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HouseBroker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IGenericRepository<Broker> _brokerRepository;
    private readonly IGenericRepository<HouseSeeker> _houseSeekerRepository;
    private readonly ILogger<AuthService> _logger;



    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, IJwtTokenService jwtTokenService, IGenericRepository<Broker> brokerRepository, IGenericRepository<HouseSeeker> houseSeekerRepository, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
        _brokerRepository = brokerRepository;
        _houseSeekerRepository = houseSeekerRepository;
        _logger = logger;
    }

    public  async Task<Response<AuthResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            _logger.LogWarning("Login failed: user with email {Email} not found", loginDto.Email);
            return Response.Failure<AuthResponseDto>(HttpContextError.NotFound("User not found."));

        }
        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed: user with email {Email} not found", loginDto.Email);
            return Response.Failure<AuthResponseDto>(HttpContextError.UnAuthorized("Invalid username or password."));

        }
        var token = await _jwtTokenService.GenerateTokenAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);
        
        _logger.LogInformation("Login successful for user {Email}", loginDto.Email);

        var authResponse = new AuthResponseDto
        {
            Token = token,
            TokenExpiration = DateTime.UtcNow.AddMinutes(60),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles.ToList()
            }
        };

        return Response.Success(authResponse);

    }

    public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: user already exists with email {Email}", registerDto.Email);
            return Response.Failure<AuthResponseDto>(HttpContextError.BadRequest("User is already registered."));
        }
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            EmailConfirmed = true,
            DateOfBirth = registerDto.DateOfBirth,
            Address = registerDto.Address,
        };
        var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);

        if (!createUserResult.Succeeded)
        {
            var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
            _logger.LogError("Registration failed for {Email}: {Errors}", registerDto.Email, errors);
            var validationError = new Error(HttpStatusCode.BadRequest, errors);
            return Response.Failure<AuthResponseDto>(validationError);
        }
        _logger.LogInformation("User {Email} created successfully", registerDto.Email);
        //Assign role to user
        await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());
        _logger.LogInformation("Role {Role} assigned to user {Email}", registerDto.Role, registerDto.Email);

        await InsertRoleSpecificRecords(user.Id, registerDto);
        
        var token = await _jwtTokenService.GenerateTokenAsync(user);
        _logger.LogInformation("Token generated for user {Email}", registerDto.Email);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber,
            Roles = new List<string> { registerDto.Role.ToString() },
        };

        var authResponse = new AuthResponseDto
        {
            Token = token,
            TokenExpiration = DateTime.UtcNow.AddMinutes(60),
            User = userDto
        };
        
        return Response.Success(authResponse);
    }

    public async Task<Response> LogoutAsync(string userId)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User with ID {UserId} logged out successfully", userId);
        return Response.Success();
    }
    
    private async Task InsertRoleSpecificRecords(Guid userId, RegisterDto registerDto)
    {
        switch (registerDto.Role)
        {
            case RoleEnum.Admin:
                // Admin: Only user record is created (already done above)
                break;

            case RoleEnum.Broker:
                // Broker: Create user + broker record
                var broker = new Broker
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LicenseNumber = registerDto.LicenseNumber ?? string.Empty,
                    AgencyId = registerDto.AgencyId,
                };

                await _brokerRepository.InsertAsync(broker);
                _logger.LogInformation("Broker entity created for user {UserId}", userId);

                break;

            case RoleEnum.HouseSeeker:
                // HouseSeeker: Create user + house seeker record
                var houseSeeker = new HouseSeeker
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                };

                await _houseSeekerRepository.InsertAsync(houseSeeker);
                _logger.LogInformation("HouseSeeker entity created for user {UserId}", userId);

                break;

            default:
                _logger.LogError("Invalid role encountered during registration: {Role}", registerDto.Role);
                throw new ArgumentException($"Invalid role: {registerDto.Role}");
        }
    }
}