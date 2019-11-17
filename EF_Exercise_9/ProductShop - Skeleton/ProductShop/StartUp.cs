namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Dtos.Export;
    using Dtos.Import;
    using Models;

    public class StartUp
    {
        private const string UsersXmlFilePath = @"../../../Datasets/users.xml";
        private const string ProductsXmlFilePath = @"../../../Datasets/products.xml";
        private const string CategoriesXmlFilePath = @"../../../Datasets/categories.xml";
        private const string CategoriesProductsXmlFilePath = @"../../../Datasets/categories-products.xml";
        private const string returnInputMessage = @"Successfully imported {0}";

        public static void Main(string[] args)
        {
            //var usersXml = File.ReadAllText(UsersXmlFilePath);
            //var productsXml = File.ReadAllText(ProductsXmlFilePath);
            //var categoriesXml = File.ReadAllText(CategoriesXmlFilePath);
            //var categoriesProductsXml = File.ReadAllText(CategoriesProductsXmlFilePath);

            using (var context = new ProductShopContext())
            {
                //Console.WriteLine(ImportUsers(context, usersXml));
                //Console.WriteLine(ImportProducts(context, productsXml));
                //Console.WriteLine(ImportCategories(context, categoriesXml));
                //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsXml));

                //Console.WriteLine(GetProductsInRange(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetCategoriesByProductsCount(context));
                Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        //1
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var attr = new XmlRootAttribute("Users");
            var serializer = new XmlSerializer(typeof(List<UserImportDto>), attr);

            var usersDto = (List<UserImportDto>)serializer
                .Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach (var userImportDto in usersDto)
            {
                var user = new User
                {
                    FirstName = userImportDto.FirstName,
                    LastName = userImportDto.LastName,
                    Age = userImportDto.Age
                };

                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return String.Format(returnInputMessage, users.Count);
        }

        //2
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var attr = new XmlRootAttribute("Products");
            var serializer = new XmlSerializer(typeof(List<ProductImportDto>), attr);

            var productsDto = (List<ProductImportDto>)serializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach (var productImportDto in productsDto)
            {
                var product = new Product
                {
                    Id = 0,
                    Name = productImportDto.Name,
                    Price = productImportDto.Price,
                    SellerId = productImportDto.SellerId,
                    BuyerId = productImportDto.BuyerId
                };

                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return String.Format(returnInputMessage, products.Count);
        }

        //3
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var attr = new XmlRootAttribute("Categories");
            var serializer = new XmlSerializer(typeof(List<CategoryImportDto>), attr);

            var categoriesDto = (List<CategoryImportDto>)serializer.Deserialize(new StringReader(inputXml));

            var categories = new List<Category>();

            foreach (var categoryImportDto in categoriesDto)
            {
                if (categoryImportDto.Name == null)
                {
                    continue;
                }

                var category = new Category
                {
                    Name = categoryImportDto.Name
                };

                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return String.Format(returnInputMessage, categories.Count);
        }

        //4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var attr = new XmlRootAttribute("CategoryProducts");
            var serializer = new XmlSerializer(typeof(List<CategoryProductsImportDto>), attr);

            var categoryProductsDto = (List<CategoryProductsImportDto>)serializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = new List<CategoryProduct>();

            var validCategoryIds = context
                .Categories
                .Select(c => c.Id)
                .ToList();

            var validProductsIds = context
                .Products
                .Select(p => p.Id)
                .ToList();


            foreach (var cp in categoryProductsDto)
            {
                if (!validCategoryIds.Contains(cp.CategoryId)
                    || !validProductsIds.Contains(cp.ProductId))
                {
                    continue;
                }

                var categoryProduct = new CategoryProduct
                {
                    CategoryId = cp.CategoryId,
                    ProductId = cp.ProductId
                };

                categoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return String.Format(returnInputMessage, categoryProducts.Count);
        }

        //5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ProductExportDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToList();

            var attr = new XmlRootAttribute("Products");
            var serializer = new XmlSerializer(typeof(List<ProductExportDto>), attr);

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });


            serializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }

        //6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .Select(u => new UserSoldProductsExportDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(p => new ProductSoldDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToList();

            var attr = new XmlRootAttribute("Users");
            var serializer = new XmlSerializer(typeof(List<UserSoldProductsExportDto>), attr);

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });


            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }

        //7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new CategoriesExportDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            var attr = new XmlRootAttribute("Categories");
            var serializer = new XmlSerializer(typeof(List<CategoriesExportDto>), attr);

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            serializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        //8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .Select(u => new UserWithProductSExportDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsForUserExportDto
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                            .Select(p => new ProductSoldDto
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p => p.Price)
                            .ToList()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToList();

            var allUsers = new AllUsersDto
            {
                Count = users.Count,
                Users = users.Take(10).ToList()
            };

            var attr = new XmlRootAttribute("Users");
            var serializer = new XmlSerializer(typeof(AllUsersDto), attr);

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            serializer.Serialize(new StringWriter(sb), allUsers, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}