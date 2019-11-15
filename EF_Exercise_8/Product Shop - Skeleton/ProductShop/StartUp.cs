namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using ProductShop.Data;
    using ProductShop.Models;
    using ModelDTOs;

    public class StartUp
    {
        private const string UsersJsonFilePath = @"../../../Datasets/users.json";
        private const string ProductsJsonFilePath = @"../../../Datasets/products.json";
        private const string CategoriesJsonFilePath = @"../../../Datasets/categories.json";
        private const string CategoriesProductsJsonFilePath = @"../../../Datasets/categories-products.json";
        private const string returnInputMessage = @"Successfully imported {0}";

        public static void Main(string[] args)
        {
            using (var context = new ProductShopContext())
            {
                var usersTextJson = File.ReadAllText(UsersJsonFilePath);
                var productsJson = File.ReadAllText(ProductsJsonFilePath);
                var categoriesJson = File.ReadAllText(CategoriesJsonFilePath);
                var categoriesProductsJson = File.ReadAllText(CategoriesProductsJsonFilePath);

                //Console.WriteLine(ImportUsers(context, usersTextJson));
                //Console.WriteLine(ImportProducts(context, productsJson));
                //Console.WriteLine(ImportCategories(context, categoriesJson));
                //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsJson));

                //Console.WriteLine(GetProductsInRange(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetCategoriesByProductsCount(context));
                Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        //1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);
            
            context.Users.AddRange(users);
            context.SaveChanges();

            return String.Format(returnInputMessage, users.Count);
        }

        //2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.Products.AddRange(products);
            context.SaveChanges();

            return String.Format(returnInputMessage, products.Count);
        }

        //3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(c => c.Name != null)
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return String.Format(returnInputMessage, categories.Count);
        }

        //4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            
            var categoriesProducts = JsonConvert.DeserializeObject<HashSet<CategoryProduct>>(inputJson);

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return String.Format(returnInputMessage, categoriesProducts.Count);
        }

        //5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToList();

            var productsInRangeJson = JsonConvert.SerializeObject(products, Formatting.Indented);

            return productsInRangeJson;
        }

        //6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new SoldProduct
                        {
                            Name = p.Name,
                            Price = p.Price,
                            BuyerFirstName = p.Buyer.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        })
                        .ToList()
                })
                .ToList();

            var soldProducts = JsonConvert.SerializeObject(users, Formatting.Indented);

            return soldProducts;
        }

        //7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new CategoryDto
                {
                    CategoryName = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AveragePrice = $"{c.CategoryProducts.Average(cp => cp.Product.Price):f2}",
                    TotalRevenue = $"{c.CategoryProducts.Sum(cp => cp.Product.Price):f2}"
                })
                .ToList();

            var productsCount = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return productsCount;
        }

        //8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                .Select(u => new UserWithProductDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsForUserWith
                        {
                            Count = u.ProductsSold.Count(p => p.Buyer != null),
                            Products = u.ProductsSold
                                .Where(p => p.Buyer != null)
                                .Select(p => new ProductForSoldProducts
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .ToList()
                        }
                })
                .ToList();

            var allUsers = new UsersDto
            {
                UsersCount = users.Count,
                Users = users
            };

            var usersJason = JsonConvert.SerializeObject(allUsers, Formatting.Indented);

            return usersJason;
        }

    }
}