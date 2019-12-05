using System;
using System.IO;
using FastFood.Data;

namespace FastFood.DataProcessor
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Dto.Export;
    using Models.Enums;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var type = Enum.Parse<OrderType>(orderType);

            var employee = context
                .Employees
                    //For Judge needs to be .ToList() here
                .ToList()
                .Where(e => e.Name == employeeName)
                .Select(e => new EmployeeExportDto
                {
                    Name = e.Name,
                    Orders = e.Orders
                        .Where(o => o.Type == type)
                        .Select(o => new OrderDetailsDto
                        {
                            Customer = o.Customer,
                            Items = o.OrderItems
                                .Select(oi => new ItemExportDto
                                {
                                    Name = oi.Item.Name,
                                    Price = oi.Item.Price,
                                    Quantity = oi.Quantity
                                })
                                .ToList(),
                            TotalPrice = o.TotalPrice
                        })
                        .OrderByDescending(o => o.TotalPrice)
                        .ThenByDescending(o => o.Items.Count)
                        .ToList(),
                    TotalMade = e.Orders
                        .Where(o => o.Type == type)
                        .Sum(p => p.TotalPrice)
                })
                .FirstOrDefault();

            var employeeJson = JsonConvert.SerializeObject(employee, Formatting.Indented);

            return employeeJson;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var searchedCategories = categoriesString
                .Split(',')
                .ToList();

            var categories = context
                .Categories
                .Where(c => searchedCategories
                    .Contains(c.Name))
                .Select(c => new CategoryExportDto
                {
                    Name = c.Name,
                    MostPopularItem = c.Items
                        .Select(i => new MostPopularItemExportDto
                        {
                            Name = i.Name,
                            TotalMade = i.Price * i.OrderItems.Sum(oi => oi.Quantity),
                            TimesSold = i.OrderItems.Sum(oi => oi.Quantity)
                        })
                        .OrderByDescending(i => i.TotalMade)
                        .ThenByDescending(i => i.TimesSold)
                        .First()
                })
                .OrderByDescending(c => c.MostPopularItem.TotalMade)
                .ThenByDescending(c => c.MostPopularItem.TimesSold)
                .ToList();



            var attr = new XmlRootAttribute("Categories");
            var serializer = new XmlSerializer(typeof(List<CategoryExportDto>), attr);

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            serializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}