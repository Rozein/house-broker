using HouseBroker.Domain.Entities;
using HouseBroker.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Infrastructure.SeedHelper;

public class Seeder
{
    public static void SeedUser(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("111787ea-adb1-484a-a8d1-d479917d2943"), //Admin
            UserName = "admin@housebroker.com",
            NormalizedUserName = "ADMIN@HOUSEBROKER.COM",
            Email = "admin@housebroker.com",
            NormalizedEmail = "ADMIN@HOUSEBROKER.COM",
            EmailConfirmed = true,
            PasswordHash =  "AQAAAAEAACcQAAAAEGS5P/Zckp606UPZS9j9uVcU8CIgdAw2N1AYZVOfIEuHfAsQ2mFOSekZFNsAuW9TVQ==", //admin@1234
            SecurityStamp = "ef831113-98d4-4505-8b34-c3c1022f8f77",
            PhoneNumber = "9876543210",
            PhoneNumberConfirmed = true,
            FirstName = "Admin",
            LastName = "HouseBroker",
            CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
            IsDeleted = false,
            Address = "Kathmandu",
            ConcurrencyStamp = "a6e1df21-e620-404e-a6a7-89c05da71e33",
        },new User
        {
            Id = Guid.Parse("222787ea-adb1-484a-a8d1-d479917d2944"), // Broker
            UserName = "broker@housebroker.com",
            NormalizedUserName = "BROKER@HOUSEBROKER.COM",
            Email = "broker@housebroker.com",
            NormalizedEmail = "BROKER@HOUSEBROKER.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAEAACcQAAAAEI3H1q/2HDAJE8XyjAzhmUwifMiBnvDMPS2IY6AU5R4vtHjgFq33oq7HwIZWaHYskg==",//broker@1234
            SecurityStamp = "bf229b22-1234-5678-9abc-def123456789",
            PhoneNumber = "9123456780",
            PhoneNumberConfirmed = true,
            FirstName = "Broker",
            LastName = "HouseBroker",
            CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
            IsDeleted = false,
            Address = "Pokhara",
            ConcurrencyStamp = "bf229b22-1234-5678-9abc-def123456789",
        }, new User
        {
            Id = Guid.Parse("333787ea-adb1-484a-a8d1-d479917d2945"), // HouseSeeker
            UserName = "seeker@housebroker.com",
            NormalizedUserName = "SEEKER@HOUSEBROKER.COM",
            Email = "seeker@housebroker.com",
            NormalizedEmail = "SEEKER@HOUSEBROKER.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAEAACcQAAAAEIGuJeVpNH1SERkiRBRgNHK66BZJFaWaCbWPX3MXi6dDyGbZZ8VCVx7k42kdVEl+rQ==",//seeker@1234
            SecurityStamp = "cf338c33-2345-6789-abcd-ef2345678901",
            PhoneNumber = "9001234567",
            PhoneNumberConfirmed = true,
            FirstName = "Seeker",
            LastName = "HouseBroker",
            CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
            IsDeleted = false,
            Address = "Biratnagar",
            ConcurrencyStamp = "cf338c33-2345-6789-abcd-ef2345678901",
        });
    }

    public static void SeedRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.Parse("9e7f0adc-ef00-4786-9990-878f480166d9"),
                Name = nameof(RoleEnum.Admin),
                NormalizedName = nameof(RoleEnum.Admin).ToUpper(),
                ConcurrencyStamp = "ede5316e-372d-40a7-8538-95c4ca18e67a",
                RoleType = RoleEnum.Admin,
                IsDeleted = false,
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                CreatedBy = Guid.Parse("111787ea-adb1-484a-a8d1-d479917d2943")
            },
            new Role
            {
                Id = Guid.Parse("144baf4b-fa9d-4b1b-83ff-c2c3f5623c9a"),
                Name = nameof(RoleEnum.Broker),
                NormalizedName = nameof(RoleEnum.Broker).ToUpper(),
                ConcurrencyStamp = "40d714a1-fa17-4a7a-b366-76abd113af4f",
                RoleType = RoleEnum.Broker,
                IsDeleted = false,
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                CreatedBy = Guid.Parse("111787ea-adb1-484a-a8d1-d479917d2943")
            },
            new Role()
            {
                Id = Guid.Parse("565d1c18-56a9-4b2e-8d65-af477cd2650a"),
                Name = nameof(RoleEnum.HouseSeeker),
                NormalizedName = nameof(RoleEnum.HouseSeeker).ToUpper(),
                ConcurrencyStamp = "337312f0-89d3-4c4a-ae66-0a05d8e2dc46",
                RoleType = RoleEnum.HouseSeeker,
                IsDeleted = false,
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                CreatedBy = Guid.Parse("111787ea-adb1-484a-a8d1-d479917d2943")
            }
        );
    }
    
    public static void SeedUserRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = Guid.Parse("111787ea-adb1-484a-a8d1-d479917d2943"),
                RoleId = Guid.Parse("9e7f0adc-ef00-4786-9990-878f480166d9"),
            }, 
            new UserRole
            {
                UserId = Guid.Parse("222787ea-adb1-484a-a8d1-d479917d2944"),
                RoleId = Guid.Parse("144baf4b-fa9d-4b1b-83ff-c2c3f5623c9a"),
            },
            new UserRole
            {
                UserId = Guid.Parse("333787ea-adb1-484a-a8d1-d479917d2945"),
                RoleId = Guid.Parse("565d1c18-56a9-4b2e-8d65-af477cd2650a"),
            }
        );
    }

    public static void SeedCity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City
            {
                Id = Guid.Parse("f4795abe-b91e-44fd-af4e-cf4c37cb52f6"),
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                IsDeleted = false,
                Name = "Kathmandu",
            },
            new City
            {
                Id = Guid.Parse("6fd08bf7-7d4f-4ce5-bc07-d29f4097c50f"),
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                IsDeleted = false,
                Name = "Bhaktapur",
            },
            new City
            {
                Id = Guid.Parse("9e1766d4-8403-4cf4-8c31-e2ebf823c809"),
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                IsDeleted = false,
                Name = "Lalitpur",
            }
        );
    }

    public static void SeedAgency(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agency>().HasData(
            new Agency
            {
                Id = Guid.Parse("b00f6552-9686-4970-90b2-244311a5b853"),
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                IsDeleted = false,
                Name = "Himalayan Agency"
            }
        );
    }

    public static void SeedBroker(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Broker>().HasData(
        
            new Broker
            {
                Id = Guid.Parse("ab6d666f-9448-45ed-9a30-d32e5eaca420"),
                CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
                IsDeleted = false,
                UserId = Guid.Parse("222787ea-adb1-484a-a8d1-d479917d2944"),
                AgencyId = Guid.Parse("b00f6552-9686-4970-90b2-244311a5b853"),
                LicenseNumber = "LI209"
            }
        );
    }

    public static void SeedHouseSeeker(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HouseSeeker>().HasData(new HouseSeeker
        {
            Id = Guid.Parse("f071d575-7072-4709-858f-413853d8969b"),
            CreationTime = new DateTime(2025, 7, 12, 10, 0, 0, DateTimeKind.Utc),
            IsDeleted = false,
            UserId = Guid.Parse("333787ea-adb1-484a-a8d1-d479917d2945"),
        });
    }
    
}