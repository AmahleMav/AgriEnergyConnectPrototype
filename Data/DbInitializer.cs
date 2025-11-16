using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AgriEnergyConnectPrototype.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgriEnergyConnectPrototype.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("DbInitializer");

            await dbContext.Database.MigrateAsync();

            // Roles
            string[] roles = { "Admin", "Farmer", "Employee" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Users (add a dedicated ENERGY vendor as Farmer)
            var seedUsers = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "admin@agriconnect.co.za",    Email = "admin@agriconnect.co.za",    FirstName = "System", LastName = "Administrator", EmailConfirmed = true },
                new ApplicationUser { UserName = "employee@agriconnect.co.za", Email = "employee@agriconnect.co.za", FirstName = "Emma",   LastName = "Employee",      EmailConfirmed = true },
                new ApplicationUser { UserName = "johnvanwyk@agrifarm.com",    Email = "johnvanwyk@agrifarm.com",    FirstName = "John",   LastName = "Van Wyk",       EmailConfirmed = true },
                new ApplicationUser { UserName = "mariamkhwanazi@greenfields.com", Email = "mariamkhwanazi@greenfields.com", FirstName = "Maria", LastName = "Mkhwanazi", EmailConfirmed = true },
                new ApplicationUser { UserName = "davidwilson@agrifields.com", Email = "davidwilson@agrifields.com", FirstName = "David",  LastName = "Wilson",       EmailConfirmed = true },

                // NEW: Energy vendor user (Farmer role, will own energy products)
                new ApplicationUser { UserName = "gabrieldaw@greentech.co.za", Email = "gabrieldaw@greentech.co.za", FirstName = "Gabriel", LastName = "Dawson", EmailConfirmed = true },
            };

            foreach (var u in seedUsers)
            {
                var existing = await userManager.FindByEmailAsync(u.Email);
                if (existing == null)
                {
                    var res = await userManager.CreateAsync(u, "Password123!");
                    if (res.Succeeded)
                    {
                        string role =
                            u.Email.Equals("admin@agriconnect.co.za", StringComparison.OrdinalIgnoreCase) ? "Admin" :
                            u.Email.EndsWith("@agriconnect.co.za", StringComparison.OrdinalIgnoreCase) && !u.Email.StartsWith("energyvendor") ? "Employee" :
                            "Farmer";

                        await userManager.AddToRoleAsync(u, role);
                    }
                }
            }

            DateTime today = DateTime.UtcNow.Date;

            async Task EnsureProfileWithProductsAsync(string email, string phone, IEnumerable<Product> products)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null) return;

                var profile = await dbContext.FarmerProfiles
                    .Include(p => p.Products)
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (profile == null)
                {
                    profile = new FarmerProfile
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = phone,
                        Products = new List<Product>()
                    };
                    dbContext.FarmerProfiles.Add(profile);
                }

                foreach (var p in products)
                {
                    bool exists = profile.Products.Any(x =>
                        x.ProductName == p.ProductName &&
                        x.Category == p.Category &&
                        x.Location == p.Location &&
                        x.Kind == p.Kind &&
                        (x.ProductionDate?.Date ?? DateTime.MinValue) == (p.ProductionDate?.Date ?? DateTime.MinValue));

                    if (!exists) profile.Products.Add(p);
                }
            }

            // ---------- Produce (local images) ----------
            // Place these files under: wwwroot/images/seed/
            // tomatoes.jpg, eggs.jpg, butternut.jpg, basil.jpg, cheese.jpg, beef.jpg, blueberries.jpg, honey.jpg, rooibos.jpg, grapes.jpg, apples.jpg, avocados.jpg, macadamia.jpg, maize.jpg

            await EnsureProfileWithProductsAsync("johnvanwyk@agrifarm.com", "082-511-6522", new[]
            {
                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Organic Tomatoes", 
                    Category="Vegetables", 
                    Location="KwaZulu-Natal",
                    ProductionDate=today, 
                    ImageUrl="/images/products/tomatoes.jpg", 
                    Unit="kg", 
                    PricePerUnit=28, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Free-Range Eggs", 
                    Category="Poultry", 
                    Location="KwaZulu-Natal",
                    ProductionDate=today, 
                    ImageUrl="/images/products/eggs.jpeg", 
                    Unit="tray", 
                    PricePerUnit=60, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Butternut Squash", 
                    Category="Vegetables", 
                    Location="KwaZulu-Natal",
                    ProductionDate=today.AddDays(-2), 
                    ImageUrl="/images/products/butternut.jpeg", 
                    Unit="kg", PricePerUnit=22, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Fresh Herbs (Basil)", 
                    Category="Herbs", 
                    Location="KwaZulu-Natal",
                    ProductionDate=today,
                    ImageUrl="/images/products/basil.jpeg", 
                    Unit="bunch", 
                    PricePerUnit=15, 
                    IsOrganic=true },
            });

            await EnsureProfileWithProductsAsync("mariamkhwanazi@greenfields.com", "071-624-5341", new[]
            {
                new Product {
                    Kind=ProductKind.Produce, 
                    ProductName="Artisanal Cheese", 
                    Category="Dairy", 
                    Location="Eastern Cape",
                    ProductionDate=today.AddDays(-7), 
                    ImageUrl="/images/products/cheese.jpg", 
                    Unit="kg", 
                    PricePerUnit=120, 
                    IsOrganic=false },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Grass-Fed Beef", 
                    Category="Meat", 
                    Location="Eastern Cape",
                    ProductionDate=today.AddDays(-28), 
                    ImageUrl="/images/products/beef.png", 
                    Unit="kg", 
                    PricePerUnit=150, 
                    IsOrganic=false },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Blueberries", 
                    Category="Fruits",
                    Location="Eastern Cape",
                    ProductionDate=today.AddDays(-1), 
                    ImageUrl="/images/products/blueberries.jpg",
                    Unit="punnet", 
                    PricePerUnit=35, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Raw Honey", 
                    Category="Honey", 
                    Location="Eastern Cape",
                    ProductionDate=today.AddDays(-3), 
                    ImageUrl="/images/products/honey.jpg", 
                    Unit="500g", 
                    PricePerUnit=75, 
                    IsOrganic=true },

                new Product { Kind=ProductKind.Produce, 
                    ProductName="Rooibos (Loose Leaf)", 
                    Category="Herbs", 
                    Location="Eastern Cape",
                    ProductionDate=today.AddDays(-10), 
                    ImageUrl="/images/products/rooibos.jpg", 
                    Unit="250g", 
                    PricePerUnit=55, 
                    IsOrganic=true },
            });

            await EnsureProfileWithProductsAsync("davidwilson@agrifields.com", "089-537-9287", new[]
            {
                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Grapes", 
                    Category="Fruits", 
                    Location="Western Cape",
                    ProductionDate=today.AddDays(-17), 
                    ImageUrl="/images/products/grapes.jpeg", 
                    Unit="kg", 
                    PricePerUnit=40, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Organic Apples", 
                    Category="Fruits", 
                    Location="Western Cape",
                    ProductionDate=today.AddDays(-5), 
                    ImageUrl="/images/products/apples.jpeg", 
                    Unit="kg", 
                    PricePerUnit=32, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Avocados", 
                    Category="Fruits", 
                    Location="Western Cape",
                    ProductionDate=today.AddDays(-2), 
                    ImageUrl="/images/products/avocados.jpg", 
                    Unit="each", 
                    PricePerUnit=15, 
                    IsOrganic=true },

                new Product { 
                    Kind=ProductKind.Produce, 
                    ProductName="Macadamia Nuts (in shell)",
                    Category="Nuts", 
                    Location="Western Cape",
                    ProductionDate=today.AddDays(-20), 
                    ImageUrl="/images/products/macadamia_nuts.jpg", 
                    Unit="kg", 
                    PricePerUnit=180, 
                    IsOrganic=false },

                new Product {
                    Kind=ProductKind.Produce, 
                    ProductName="Maize (Grain)", 
                    Category="Grains",
                    Location="Western Cape",
                    ProductionDate=today.AddDays(-30), 
                    ImageUrl="/images/products/maize.jpg", 
                    Unit="50kg bag", 
                    PricePerUnit=380, 
                    IsOrganic=false },
            });

            // ---------- Energy Solutions owned by ENERGY VENDOR (Farmer) ----------
            // Place under wwwroot/images/seed:
            // solar_irrigation_5kw.jpg, coldroom_solar_10kw.jpg, wind_20kw.jpg, biogas_8m3.jpg, battery_15kwh.jpg, borehole_pump_dc.jpg

            await EnsureProfileWithProductsAsync("gabrieldaw@greentech.co.za", "083-432-1911", new[]
            {
                new Product {
                    Kind=ProductKind.EnergySolution, ProductName="Solar Irrigation Kit 5 kW",
                    Category="Solar", Location="National", ImageUrl="/images/products/solar_irrigation_system.jpeg",
                    VendorName="SunHarvest SA", EnergyType="Solar", PowerkW=5, SuitableFor="Irrigation",
                    PriceZar=145000
                },
                new Product {
                    Kind=ProductKind.EnergySolution, ProductName="Cold-Room Solar System 10 kW",
                    Category="Solar", Location="National", ImageUrl="/images/products/cold_room_solar.jpeg",
                    VendorName="CapeSolar Farms", EnergyType="Solar", PowerkW=10, SuitableFor="Cold storage",
                    PriceZar=465000
                },
                new Product {
                    Kind=ProductKind.EnergySolution, ProductName="Wind Turbine 20 kW (Farm Pack)",
                    Category="Wind", Location="National", ImageUrl="/images/products/wind_turbine.jpeg",
                    VendorName="GreenCape Wind", EnergyType="Wind", PowerkW=20, SuitableFor="Mixed farm loads",
                    PriceZar=980000
                },
                new Product {
                    Kind=ProductKind.EnergySolution, ProductName="Biogas Digester",
                    Category="Biogas", Location="National", ImageUrl="/images/products/biogas_digester.jpg",
                    VendorName="AgriBiogas Africa", EnergyType="Biogas", SuitableFor="Dairy/Livestock",
                    PriceZar=210000
                },
                new Product {
                    Kind=ProductKind.EnergySolution, ProductName="Lithium Battery Bank 15 kWh",
                    Category="Storage", Location="National", ImageUrl="/images/products/lithium_battery_bank.png",
                    VendorName="EcoStore Energy", EnergyType="Storage", SuitableFor="All farm types",
                    PriceZar=155000
                },
                new Product {
                    Kind=ProductKind.EnergySolution, ProductName="Solar Borehole Pump (Deep-Well)",
                    Category="Pump", Location="National", ImageUrl="/images/products/solar_borehole_pump.jpg",
                    VendorName="AquaSun Pumps", EnergyType="Solar", PowerkW=2.2, SuitableFor="Irrigation / stock water",
                    PriceZar=128000
                },
            });

            await dbContext.SaveChangesAsync();
            logger?.LogInformation("Seeding completed: produce + energy solutions with local images and energy vendor user.");
        }
    }
}
