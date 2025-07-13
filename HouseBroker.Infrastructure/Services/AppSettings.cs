using HouseBroker.Application.Interface;
using Microsoft.Extensions.Configuration;

namespace HouseBroker.Infrastructure.Services;

public class AppSettings : IAppSettings
{
    private readonly IConfiguration _configuration;

    public AppSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string ImageBaseUrl => _configuration["ImageSettings:BaseUrl"]!;
}