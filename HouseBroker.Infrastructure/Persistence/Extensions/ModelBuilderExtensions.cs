

using System.Linq.Expressions;
using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using HouseBroker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /*
    Summary:
    This code automatically applies a global filter to every entity that supports soft delete (marked with ISoftDeleteEntity).
    The filter ensures that when querying these entities, only records where IsDeleted is false (not deleted) will be returned.

    This means you never have to manually add ".Where(IsDeleted == false)" to your queries — EF Core will do it for you globally.

    In simpler terms, soft-deleted records become invisible to your application automatically.
    */
    public static void ApplyGlobalFilters(this ModelBuilder modelBuilder)
    {
        // Get the interface type that marks entities with soft delete capability
        var softDeleteInterface = typeof(ISoftDeleteEntity);

        // Loop through all entity types registered in the EF Core model
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Get the CLR type (class) of the entity
            var clrType = entityType.ClrType;

            // Check if this entity implements the soft delete interface
            if (softDeleteInterface.IsAssignableFrom(clrType))
            {
                // We will build a lambda expression to represent the filter:
                // e => EF.Property<bool>(e, "IsDeleted") == false

                // Create a parameter expression representing the entity instance in the lambda (named "e")
                var parameter = Expression.Parameter(clrType, "e");

                // Get the generic EF.Property<bool> method info used to access shadow or regular properties dynamically
                var propertyMethod = typeof(EF)
                    .GetMethod(nameof(EF.Property), new[] { typeof(object), typeof(string) })!
                    .MakeGenericMethod(typeof(bool));

                // Build a method call expression to EF.Property<bool>(e, "IsDeleted")
                var isDeletedPropertyAccess = Expression.Call(
                    propertyMethod,
                    parameter,
                    Expression.Constant(nameof(ISoftDeleteEntity.IsDeleted)));

                // Build a comparison expression: EF.Property<bool>(e, "IsDeleted") == false
                var condition = Expression.Equal(isDeletedPropertyAccess, Expression.Constant(false));

                // Create the lambda expression combining parameter and condition: e => EF.Property<bool>(e, "IsDeleted") == false
                var lambda = Expression.Lambda(condition, parameter);

                // Apply the lambda as a global query filter on the entity type, so all queries automatically exclude soft deleted entities
                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }
        }

    }
    public static void FluentValidation(this ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            
        });
        
        modelBuilder.Entity<Property>(builder =>
        {
            builder.Property(p => p.PropertyType)
                .HasConversion<string>()
                .IsRequired();
        });
        
        modelBuilder.Entity<Location>(builder =>
        {
            builder.HasIndex(l => new { l.CityId, l.Area,l.PostalCode }).IsUnique();
        });

    }
}

