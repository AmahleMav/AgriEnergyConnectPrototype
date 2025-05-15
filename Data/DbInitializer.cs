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
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed roles
            string[] roles = { "Admin", "Farmer", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created.");
                }
            }

            // Seed Admin
            var adminEmail = "admin@agriconnect.co.za";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Admin user seeded.");
                }
                else
                {
                    Console.WriteLine($"Failed to seed admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Seed Farmers with Profiles and Products
            var farmers = new List<(IdentityUser user, FarmerProfile profile)>
            {
                (
                    new IdentityUser { UserName = "johnvanwyk@agrifarm.com", Email = "johnvanwyk@agrifarm.com", EmailConfirmed = true },
                    new FarmerProfile
                    {
                        FirstName = "John",
                        LastName = "Van Wyk",
                        PhoneNumber = "082-511-6522",
                        Products = new List<Product>
                        {
                            new Product { ProductName = "Organic Tomatoes", Category = "Vegetables", Location = "Kwa-Zulu Natal", ProductionDate = DateTime.Now },
                            new Product { ProductName = "Free-Range Eggs", Category = "Poultry", Location = "Kwa-Zulu Natal", ProductionDate = DateTime.Now },
                            new Product { ProductName = "Organic Carrots", Category = "Vegetables", Location = "Kwa-Zulu Natal", ProductionDate = DateTime.Now }
                        }
                    }
                ),
                (
                    new IdentityUser { UserName = "mariamkhwanazi@greenfields.com", Email = "mariamkhwanazi@greenfields.com", EmailConfirmed = true },
                    new FarmerProfile
                    {
                        FirstName = "Maria",
                        LastName = "Mkhwanazi",
                        PhoneNumber = "071-624-5341",
                        Products = new List<Product>
                        {
                            new Product { ProductName = "Artisanal Cheese", Category = "Dairy", Location = "Eastern Cape", ProductionDate = DateTime.Now.AddDays(-7) },
                            new Product { ProductName = "Grass-Fed Beef", Category = "Meat", Location = "Eastern Cape", ProductionDate = DateTime.Now.AddDays(-28) },
                            new Product { ProductName = "Goat Milk", Category = "Dairy", Location = "Eastern Cape", ProductionDate = DateTime.Now }
                        }
                    }
                ),
                (
                    new IdentityUser { UserName = "davidwilson@agrifields", Email = "davidwilson@agrifields.com", EmailConfirmed = true },
                    new FarmerProfile
                    {
                        FirstName = "David",
                        LastName = "Wilson",
                        PhoneNumber = "089-537-9287",
                        Products = new List<Product>
                        {
                            new Product { ProductName = "Grapes", Category = "Fruit", Location = "Western Cape", ProductionDate = DateTime.Now.AddDays(-17) },
                            new Product { ProductName = "Organic Apples", Category = "Fruits", Location = "Western Cape", ProductionDate = DateTime.Now.AddDays(-5) },
                            new Product { ProductName = "Organic Potatoes", Category = "Vegetables", Location = "Western Cape", ProductionDate = DateTime.Now.AddDays(-30) }
                        }
                    }
                )
            };

            foreach (var (user, profile) in farmers)
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var createResult = await userManager.CreateAsync(user, "Farmer123!");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Farmer");
                        Console.WriteLine($"Farmer user {user.Email} created.");

                        
                        profile.UserId = user.Id;
                        profile.Email = user.Email;
                        dbContext.FarmerProfiles.Add(profile);
                        await dbContext.SaveChangesAsync(); 

                        var savedProfile = await dbContext.FarmerProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
                        if (savedProfile != null)
                        {
                            foreach (var product in profile.Products)
                            {
                                if (!dbContext.Products.Any(p => p.ProductName == product.ProductName && p.FarmerId == savedProfile.FarmerId))
                                {
                                    product.FarmerId = savedProfile.FarmerId;
                                    dbContext.Products.Add(product);
                                }
                            }
                            await dbContext.SaveChangesAsync(); 
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create farmer {user.Email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    }
                }
            }



            // Seed Employee
            var employeeEmail = "employee@agriconnect.co.za";
            var employeeUser = await userManager.FindByEmailAsync(employeeEmail);
            if (employeeUser == null)
            {
                employeeUser = new IdentityUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(employeeUser, "Employee123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                    Console.WriteLine("Employee user seeded.");
                }
                else
                {
                    Console.WriteLine($"Failed to seed employee: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            await dbContext.SaveChangesAsync();
            Console.WriteLine("Database seeding completed.");
        }
    }
}
