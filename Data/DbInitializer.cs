using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AgriEnergyConnectPrototype.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnectPrototype.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure roles exist
            string[] roles = { "Admin", "Farmer", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created.");
                }
            }

            // Special users to seed
            var seedUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "admin@agriconnect.co.za",
                    Email = "admin@agriconnect.co.za",
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "employee@agriconnect.co.za",
                    Email = "employee@agriconnect.co.za",
                    FirstName = "Emma",
                    LastName = "Employee",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "johnvanwyk@agrifarm.com",
                    Email = "johnvanwyk@agrifarm.com",
                    FirstName = "John",
                    LastName = "Van Wyk",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "mariamkhwanazi@greenfields.com",
                    Email = "mariamkhwanazi@greenfields.com",
                    FirstName = "Maria",
                    LastName = "Mkhwanazi",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "davidwilson@agrifields.com",
                    Email = "davidwilson@agrifields.com",
                    FirstName = "David",
                    LastName = "Wilson",
                    EmailConfirmed = true
                }
            };

            foreach (var user in seedUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(user, "Password123!");
                    if (result.Succeeded)
                    {
                        // Auto-assign roles based on email rules
                        string role;
                        if (user.Email.Equals("admin@agriconnect.co.za", StringComparison.OrdinalIgnoreCase))
                        {
                            role = "Admin";
                        }
                        else if (user.Email.EndsWith("@agriconnect.co.za", StringComparison.OrdinalIgnoreCase))
                        {
                            role = "Employee";
                        }
                        else
                        {
                            role = "Farmer";
                        }

                        await userManager.AddToRoleAsync(user, role);
                        Console.WriteLine($"{role} user {user.Email} created.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Example Farmer Profiles + Products
            var john = await userManager.FindByEmailAsync("johnvanwyk@agrifarm.com");
            if (john != null && !dbContext.FarmerProfiles.Any(p => p.UserId == john.Id))
            {
                var profile = new FarmerProfile
                {
                    UserId = john.Id,
                    Email = john.Email,
                    FirstName = "John",
                    LastName = "Van Wyk",
                    PhoneNumber = "082-511-6522",
                    Products = new List<Product>
                    {
                        new Product { ProductName = "Organic Tomatoes", Category = "Vegetables", Location = "Kwa-Zulu Natal", ProductionDate = DateTime.Now },
                        new Product { ProductName = "Free-Range Eggs", Category = "Poultry", Location = "Kwa-Zulu Natal", ProductionDate = DateTime.Now },
                        new Product { ProductName = "Organic Carrots", Category = "Vegetables", Location = "Kwa-Zulu Natal", ProductionDate = DateTime.Now }
                    }
                };
                dbContext.FarmerProfiles.Add(profile);
            }

            var maria = await userManager.FindByEmailAsync("mariamkhwanazi@greenfields.com");
            if (maria != null && !dbContext.FarmerProfiles.Any(p => p.UserId == maria.Id))
            {
                var profile = new FarmerProfile
                {
                    UserId = maria.Id,
                    Email = maria.Email,
                    FirstName = "Maria",
                    LastName = "Mkhwanazi",
                    PhoneNumber = "071-624-5341",
                    Products = new List<Product>
                    {
                        new Product { ProductName = "Artisanal Cheese", Category = "Dairy", Location = "Eastern Cape", ProductionDate = DateTime.Now.AddDays(-7) },
                        new Product { ProductName = "Grass-Fed Beef", Category = "Meat", Location = "Eastern Cape", ProductionDate = DateTime.Now.AddDays(-28) },
                        new Product { ProductName = "Goat Milk", Category = "Dairy", Location = "Eastern Cape", ProductionDate = DateTime.Now }
                    }
                };
                dbContext.FarmerProfiles.Add(profile);
            }

            var david = await userManager.FindByEmailAsync("davidwilson@agrifields.com");
            if (david != null && !dbContext.FarmerProfiles.Any(p => p.UserId == david.Id))
            {
                var profile = new FarmerProfile
                {
                    UserId = david.Id,
                    Email = david.Email,
                    FirstName = "David",
                    LastName = "Wilson",
                    PhoneNumber = "089-537-9287",
                    Products = new List<Product>
                    {
                        new Product { ProductName = "Grapes", Category = "Fruit", Location = "Western Cape", ProductionDate = DateTime.Now.AddDays(-17) },
                        new Product { ProductName = "Organic Apples", Category = "Fruit", Location = "Western Cape", ProductionDate = DateTime.Now.AddDays(-5) },
                        new Product { ProductName = "Organic Potatoes", Category = "Vegetables", Location = "Western Cape", ProductionDate = DateTime.Now.AddDays(-30) }
                    }
                };
                dbContext.FarmerProfiles.Add(profile);
            }

            await dbContext.SaveChangesAsync();
            Console.WriteLine("Database seeding completed.");
        }
    }
}
