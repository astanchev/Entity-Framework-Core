namespace CarDealer
{
    using Data;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using DTO;

    public class StartUp
    {
        private const string CarsJsonFilePath = @"../../../Datasets/cars.json";
        private const string CustomersJsonFilePath = @"../../../Datasets/customers.json";
        private const string PartsJsonFilePath = @"../../../Datasets/parts.json";
        private const string SalesJsonFilePath = @"../../../Datasets/sales.json";
        private const string SuppliersJsonFilePath = @"../../../Datasets/suppliers.json";
        private const string returnInputMessage = @"Successfully imported {0}.";
        public static void Main(string[] args)
        {
            using (var context = new CarDealerContext())
            {
                var carsTextJson = File.ReadAllText(CarsJsonFilePath);
                var customersTextJson = File.ReadAllText(CustomersJsonFilePath);
                var partsTextJson = File.ReadAllText(PartsJsonFilePath);
                var salesTextJson = File.ReadAllText(SalesJsonFilePath);
                var suppliersTextJson = File.ReadAllText(SuppliersJsonFilePath);

                //Console.WriteLine(ImportSuppliers(context, suppliersTextJson));
                //Console.WriteLine(ImportParts(context, partsTextJson));
                //Console.WriteLine(ImportCars(context, carsTextJson));
                //Console.WriteLine(ImportCustomers(context, customersTextJson));
                //Console.WriteLine(ImportSales(context, salesTextJson));

                //Console.WriteLine(GetOrderedCustomers(context));
                //Console.WriteLine(GetCarsFromMakeToyota(context));
                //Console.WriteLine(GetLocalSuppliers(context));
                //Console.WriteLine(GetCarsWithTheirListOfParts(context));
                //Console.WriteLine(GetTotalSalesByCustomer(context));
                Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        //9
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return String.Format(returnInputMessage, suppliers.Count);
        }

        //10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var validSupplierIds = context
                .Suppliers
                .Select(s => s.Id)
                .ToList();

            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                .Where(p => validSupplierIds.Contains(p.SupplierId))
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return String.Format(returnInputMessage, parts.Count);
        }

        //11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsJson = JsonConvert.DeserializeObject<List<ImportCarDto>>(inputJson);

            var validPartIds = context
                .Parts
                .Select(p => p.Id)
                .ToHashSet();

            var carParts = new HashSet<PartCar>();

            foreach (var car in carsJson)
            {
                var carToAdd = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                context.Cars.Add(carToAdd);

                foreach (var partsCarId in car.PartsId)
                {

                    if (validPartIds.Contains(partsCarId))
                    {
                        var carPartToAdd = new PartCar
                        {
                            CarId = carToAdd.Id,
                            PartId = partsCarId
                        };

                        if (carToAdd.PartCars.FirstOrDefault(p => p.PartId == partsCarId
                                                                  && p.CarId == carToAdd.Id) == null)
                        {
                            carToAdd.PartCars.Add(carPartToAdd);
                            context.PartCars.Add(carPartToAdd);
                        }
                    }
                }
            }

            context.SaveChanges();

            return String.Format(returnInputMessage, carsJson.Count);
        }

        //12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return String.Format(returnInputMessage, customers.Count);
        }

        //13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return String.Format(returnInputMessage, sales.Count);
        }

        //14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new CustomerDto
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();


            var customersExport = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return customersExport;
        }

        //15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportCarDto
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            var carsExport = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return carsExport;
        }

        //16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var suppliersExport = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return suppliersExport;
        }

        //17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Select(c => new DetailedCarDto
                {
                    Car = new CarDto
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    Parts = c.PartCars
                        .Select(p => new PartDto
                        {
                            Name = p.Part.Name,
                            Price = $"{p.Part.Price:f2}"
                        })
                        .ToList()
                })
                .ToList();

            var carsExport = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return carsExport;
        }

        //18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new CustomerSaleDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales
                        .Sum(s => s.Car.PartCars
                            .Sum(pc => pc.Part.Price))
                })
                .ToList();

            var customersExport = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return customersExport;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(s => new SaleDto
                {
                    Car = new CarDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = $"{s.Discount:f2}",
                    Price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):f2}",
                    PriceWithDiscount = $"{s.Car.PartCars.Sum(pc => pc.Part.Price) - s.Car.PartCars.Sum(pc => pc.Part.Price) * (s.Discount / 100m):f2}"
                })
                .Take(10)
                .ToList();

            var salesExport = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return salesExport;
        }
    }
}