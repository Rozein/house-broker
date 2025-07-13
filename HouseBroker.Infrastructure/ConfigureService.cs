using System.Reflection;
using HouseBroker.Application;
using HouseBroker.Application.CurrentUserService;
using HouseBroker.Application.Interface;
using HouseBroker.Application.Interface.DIRegistration;
using HouseBroker.Application.Interface.Services;
using HouseBroker.Domain.Entities;
using HouseBroker.Infrastructure.Interceptors;
using HouseBroker.Infrastructure.Persistence;
using HouseBroker.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HouseBroker.Infrastructure;

public static class ConfigureService
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddHttpContextAccessor();
        // Registers the SaveChangesInterceptor to hook into EF Core's SaveChanges pipeline.
        // This allows for cross-cutting concerns such as auditing, soft deletes, or automatic metadata updates
        // to be handled transparently whenever entities are saved to the database.
        services.AddScoped<SaveChangesInterceptor>();        
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        // Automatically registers services based on marker interfaces (e.g., IScopedDependency, ISingletonDependency, ITransientDependency)
        // by scanning the current assembly and applying the appropriate lifetime to each service.
        // This promotes cleaner dependency registration and adheres to the dependency inversion principle.
        RegisterMarkerServices(services, Assembly.GetExecutingAssembly());
        
        services.AddSingleton<IImageStorageService>(provider =>
            new LocalImageStorageService(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"),
                configuration.GetValue<string>("ImageSettings:BaseUrl"))); 
        
        // Identity Configuration
        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<HouseBrokerDbContext>()
            .AddDefaultTokenProviders()
            .AddRoles<Role>();


        return services; //Return for method chaining
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<HouseBrokerDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(HouseBrokerDbContext).Assembly.FullName);
            });

            options.EnableSensitiveDataLogging(); //  Optional for development only
        });
    }
    
    private static void RegisterMarkerServices(IServiceCollection services, Assembly assembly)
    {
        var allTypes = assembly.GetTypes();

        var markerInterfaces = new[]
        {
            typeof(IScopedDependency),
            typeof(ISingletonDependency),
            typeof(ITransientDependency)
        };

        var implementations = allTypes
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(impl => impl.GetInterfaces()
                .Where(i => markerInterfaces.Any(marker => marker.IsAssignableFrom(i)) && !markerInterfaces.Contains(i))
                .Select(serviceInterface => new { serviceInterface, implementation = impl }))
            .ToList();

        foreach (var pair in implementations)
        {
            if (typeof(ISingletonDependency).IsAssignableFrom(pair.serviceInterface))
                services.AddSingleton(pair.serviceInterface, pair.implementation);
            else if (typeof(ITransientDependency).IsAssignableFrom(pair.serviceInterface))
                services.AddTransient(pair.serviceInterface, pair.implementation);
            else
                services.AddScoped(pair.serviceInterface, pair.implementation); // Default: Scoped
        }
    }

}