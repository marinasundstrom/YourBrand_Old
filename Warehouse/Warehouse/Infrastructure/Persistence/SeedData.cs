﻿using YourBrand.Warehouse.Domain.Entities;

namespace YourBrand.Warehouse.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            if (!context.Items.Any())
            {
                var person = new Item("John", "Doe", "3234234234")
                {
                    PhoneHome = null,
                    PhoneMobile = "072423123",
                    Email = "test@test.com",
                };

                /*
                person.AddAddress(new Address("foo") {
                    Type = Domain.Enums.AddressType.Billing,
                    Thoroughfare = "Baker Street",
                    SubPremises = null,
                    Premises = "42",
                    PostalCode = "4534 23",
                    Locality = "Testville",
                    SubAdministrativeArea = "Sub",
                    AdministrativeArea =  "Area",
                    Country = "Testland"
                });
                */

                context.Items.Add(person);

                await context.SaveChangesAsync();
            }
        }
    }
}