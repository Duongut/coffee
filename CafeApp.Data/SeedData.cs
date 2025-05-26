using CafeApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Admin User
            await SeedAdminUserAsync(userManager);

            // Seed Categories
            await SeedCategoriesAsync(context);

            // Seed Products
            await SeedProductsAsync(context);

            // Seed Sample Orders
            await SeedSampleOrdersAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Staff", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@cafeapp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Coffee", Description = "Hot and cold coffee beverages", IsActive = true },
                new Category { Name = "Tea", Description = "Various tea selections", IsActive = true },
                new Category { Name = "Pastries", Description = "Fresh baked goods and pastries", IsActive = true },
                new Category { Name = "Sandwiches", Description = "Delicious sandwiches and wraps", IsActive = true },
                new Category { Name = "Desserts", Description = "Sweet treats and desserts", IsActive = true },
                new Category { Name = "Smoothies", Description = "Fresh fruit smoothies and shakes", IsActive = true }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var categories = await context.Categories.ToListAsync();
            var coffeeCategory = categories.First(c => c.Name == "Coffee");
            var teaCategory = categories.First(c => c.Name == "Tea");
            var pastryCategory = categories.First(c => c.Name == "Pastries");
            var sandwichCategory = categories.First(c => c.Name == "Sandwiches");
            var dessertCategory = categories.First(c => c.Name == "Desserts");
            var smoothieCategory = categories.First(c => c.Name == "Smoothies");

            var products = new List<Product>
            {
                // Coffee
                new Product { Name = "Espresso", Description = "Rich and bold espresso shot", Price = 2.50m, CategoryId = coffeeCategory.Id, IsAvailable = true, IsFeatured = true },
                new Product { Name = "Americano", Description = "Espresso with hot water", Price = 3.00m, CategoryId = coffeeCategory.Id, IsAvailable = true },
                new Product { Name = "Cappuccino", Description = "Espresso with steamed milk and foam", Price = 4.00m, CategoryId = coffeeCategory.Id, IsAvailable = true, IsFeatured = true },
                new Product { Name = "Latte", Description = "Espresso with steamed milk", Price = 4.50m, CategoryId = coffeeCategory.Id, IsAvailable = true },
                new Product { Name = "Mocha", Description = "Espresso with chocolate and steamed milk", Price = 5.00m, CategoryId = coffeeCategory.Id, IsAvailable = true },
                new Product { Name = "Iced Coffee", Description = "Cold brew coffee over ice", Price = 3.50m, CategoryId = coffeeCategory.Id, IsAvailable = true },

                // Tea
                new Product { Name = "Green Tea", Description = "Fresh green tea leaves", Price = 2.00m, CategoryId = teaCategory.Id, IsAvailable = true },
                new Product { Name = "Earl Grey", Description = "Classic black tea with bergamot", Price = 2.50m, CategoryId = teaCategory.Id, IsAvailable = true },
                new Product { Name = "Chamomile", Description = "Relaxing herbal tea", Price = 2.50m, CategoryId = teaCategory.Id, IsAvailable = true },
                new Product { Name = "Chai Latte", Description = "Spiced tea with steamed milk", Price = 4.00m, CategoryId = teaCategory.Id, IsAvailable = true, IsFeatured = true },

                // Pastries
                new Product { Name = "Croissant", Description = "Buttery, flaky pastry", Price = 3.00m, CategoryId = pastryCategory.Id, IsAvailable = true },
                new Product { Name = "Blueberry Muffin", Description = "Fresh blueberry muffin", Price = 2.75m, CategoryId = pastryCategory.Id, IsAvailable = true, IsFeatured = true },
                new Product { Name = "Danish", Description = "Sweet pastry with fruit filling", Price = 3.25m, CategoryId = pastryCategory.Id, IsAvailable = true },
                new Product { Name = "Bagel", Description = "Fresh baked bagel with cream cheese", Price = 2.50m, CategoryId = pastryCategory.Id, IsAvailable = true },

                // Sandwiches
                new Product { Name = "Club Sandwich", Description = "Turkey, bacon, lettuce, tomato", Price = 8.50m, CategoryId = sandwichCategory.Id, IsAvailable = true },
                new Product { Name = "Grilled Cheese", Description = "Melted cheese on toasted bread", Price = 6.00m, CategoryId = sandwichCategory.Id, IsAvailable = true },
                new Product { Name = "BLT", Description = "Bacon, lettuce, and tomato sandwich", Price = 7.00m, CategoryId = sandwichCategory.Id, IsAvailable = true },
                new Product { Name = "Veggie Wrap", Description = "Fresh vegetables in a tortilla wrap", Price = 7.50m, CategoryId = sandwichCategory.Id, IsAvailable = true },

                // Desserts
                new Product { Name = "Chocolate Cake", Description = "Rich chocolate layer cake", Price = 4.50m, CategoryId = dessertCategory.Id, IsAvailable = true, IsFeatured = true },
                new Product { Name = "Cheesecake", Description = "Creamy New York style cheesecake", Price = 5.00m, CategoryId = dessertCategory.Id, IsAvailable = true },
                new Product { Name = "Tiramisu", Description = "Italian coffee-flavored dessert", Price = 5.50m, CategoryId = dessertCategory.Id, IsAvailable = true },
                new Product { Name = "Cookies", Description = "Fresh baked chocolate chip cookies", Price = 2.00m, CategoryId = dessertCategory.Id, IsAvailable = true },

                // Smoothies
                new Product { Name = "Berry Smoothie", Description = "Mixed berries with yogurt", Price = 5.50m, CategoryId = smoothieCategory.Id, IsAvailable = true },
                new Product { Name = "Mango Smoothie", Description = "Fresh mango with coconut milk", Price = 5.75m, CategoryId = smoothieCategory.Id, IsAvailable = true },
                new Product { Name = "Green Smoothie", Description = "Spinach, apple, and banana", Price = 6.00m, CategoryId = smoothieCategory.Id, IsAvailable = true },
                new Product { Name = "Protein Smoothie", Description = "Banana, peanut butter, and protein powder", Price = 6.50m, CategoryId = smoothieCategory.Id, IsAvailable = true }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSampleOrdersAsync(ApplicationDbContext context)
        {
            if (await context.Orders.AnyAsync())
                return;

            var products = await context.Products.ToListAsync();
            var cappuccino = products.First(p => p.Name == "Cappuccino");
            var muffin = products.First(p => p.Name == "Blueberry Muffin");

            var sampleOrder = new Order
            {
                OrderNumber = "ORD-20250524-0001",
                TableNumber = 5,
                CustomerName = "John Doe",
                CustomerPhone = "555-0123",
                Status = Core.Enums.OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                CompletedAt = DateTime.UtcNow.AddHours(-1),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = cappuccino.Id,
                        Quantity = 2,
                        UnitPrice = cappuccino.Price,
                        TotalPrice = cappuccino.Price * 2,
                        Notes = "Extra hot"
                    },
                    new OrderItem
                    {
                        ProductId = muffin.Id,
                        Quantity = 1,
                        UnitPrice = muffin.Price,
                        TotalPrice = muffin.Price * 1
                    }
                }
            };

            sampleOrder.SubTotal = sampleOrder.OrderItems.Sum(oi => oi.TotalPrice);
            sampleOrder.TotalAmount = sampleOrder.SubTotal;

            context.Orders.Add(sampleOrder);
            await context.SaveChangesAsync();
        }
    }
}
