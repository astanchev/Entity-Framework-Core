using System;
using FastFood.Data;
namespace FastFood.DataProcessor
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Dto.Import;
    using Models;
    using Models.Enums;
    using Newtonsoft.Json;

    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var employeesDto = JsonConvert.DeserializeObject<List<EmployeeImportDto>>(jsonString);

            StringBuilder sb = new StringBuilder();

            var validEmployees = new List<Employee>();

            foreach (var dto in employeesDto)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var employee = new Employee
                {
                    Name = dto.Name,
                    Age = dto.Age
                };

                var position = context
                    .Positions
                    .FirstOrDefault(p => p.Name == dto.Position);

                if (position == null)
                {
                    position = new Position{Name = dto.Position};
                    context.Positions.Add(position);
                    context.SaveChanges();
                }

                employee.Position = position;
                validEmployees.Add(employee);
                sb.AppendLine(String.Format(SuccessMessage, employee.Name));
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            var itemsDto = JsonConvert.DeserializeObject<List<ItemImportDto>>(jsonString);

            StringBuilder sb = new StringBuilder();

            var validItems = new List<Item>();

            var validItemNames = context
                .Items
                .Select(i => i.Name)
                .ToList();

            foreach (var dto in itemsDto)
            {
                if (!IsValid(dto) || validItemNames.Contains(dto.Name))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var item = new Item
                {
                    Name = dto.Name,
                    Price = dto.Price
                };

                var category = context
                    .Categories
                    .FirstOrDefault(c => c.Name == dto.Category);

                if (category == null)
                {
                    category = new Category {Name = dto.Category};
                    context.Categories.Add(category);
                    context.SaveChanges();
                }

                item.Category = category;

                validItems.Add(item);
                validItemNames.Add(item.Name);

                sb.AppendLine(String.Format(SuccessMessage, item.Name));
            }

            context.Items.AddRange(validItems);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            var attr = new XmlRootAttribute("Orders");
            var serializer = new XmlSerializer(typeof(List<OrderImportDto>), attr);

            StringBuilder sb = new StringBuilder();

            var validOrders = new List<Order>();
            var validItemsNames = context
                .Items
                .Select(i => i.Name)
                .ToList();

            using (StringReader reader = new StringReader(xmlString))
            {
                var ordersDto = (List<OrderImportDto>)serializer.Deserialize(reader);

                foreach (var dto in ordersDto)
                {
                    var employee = context
                        .Employees
                        .FirstOrDefault(e => e.Name == dto.Employee);

                    if (!IsValid(dto) 
                        || !dto.Items.All(IsValid)
                        || employee == null
                        || !dto.Items.All(i => validItemsNames.Contains(i.Name)))
                    {
                        sb.AppendLine(FailureMessage);
                        continue;
                    }
                    
                    var order = new Order
                    {
                        Customer = dto.Customer,
                        DateTime = DateTime.ParseExact(dto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                        Type = dto.Type,
                        Employee = employee
                    };

                    foreach (var item in dto.Items)
                    {
                        var orderItemToAdd = new OrderItem
                        {
                            Order = order,
                            Item = context.Items.First(i => i.Name == item.Name),
                            Quantity = item.Quantity
                        };

                        order.OrderItems.Add(orderItemToAdd);
                    }

                    validOrders.Add(order);

                    sb.AppendLine($"Order for {order.Customer} on {order.DateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} added");
                }
            }

            context.Orders.AddRange(validOrders);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            var result = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return result;
        }
    }
}