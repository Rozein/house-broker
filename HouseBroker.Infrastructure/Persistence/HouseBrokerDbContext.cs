using HouseBroker.Domain.Entities;
using HouseBroker.Infrastructure.Interceptors;
using HouseBroker.Infrastructure.Persistence.Extensions;
using HouseBroker.Infrastructure.SeedHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Infrastructure.Persistence;

public class HouseBrokerDbContext: IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly SaveChangesInterceptor _saveChangesInterceptor;
    public HouseBrokerDbContext(DbContextOptions<HouseBrokerDbContext> options, SaveChangesInterceptor saveChangesInterceptor)
        : base(options)
    {
        _saveChangesInterceptor = saveChangesInterceptor;
    }

    public DbSet<Agency> Agency { get; set; }
    public DbSet<Broker> Broker { get; set; }
    public DbSet<HouseSeeker> HouseSeeker { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Property> Property { get; set; }
    public DbSet<PropertyAttachments> PropertyAttachments { get; set; }
    public DbSet<City> City { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HouseBrokerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
        
        Seeder.SeedUser(modelBuilder);
        Seeder.SeedRole(modelBuilder);
        Seeder.SeedUserRoles(modelBuilder);
        Seeder.SeedCity(modelBuilder);
        Seeder.SeedAgency(modelBuilder);
        Seeder.SeedBroker(modelBuilder);
        Seeder.SeedHouseSeeker(modelBuilder);
        modelBuilder.ApplyGlobalFilters();
        modelBuilder.FluentValidation();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_saveChangesInterceptor);
    }
}