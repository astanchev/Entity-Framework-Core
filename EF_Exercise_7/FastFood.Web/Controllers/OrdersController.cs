namespace FastFood.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Models;
    using Models.Enums;
    using ViewModels.Orders;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = this.context.Items.OrderBy(i => i.Id).Select(i => i.Id).ToList(),
                ItemsNames = this.context.Items.OrderBy(i => i.Id).Select(i => i.Name).ToList(),
                Employees = this.context.Employees.OrderBy(e => e.Id).Select(e => e.Id).ToList(),
                EmployeesNames = this.context.Employees.OrderBy(e => e.Id).Select(e => e.Name).ToList()
            };

            return this.View(viewOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        { 
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var employee = this.context
                .Employees
                .FirstOrDefault(e => e.Name == model.EmployeeName);
            var order = this.mapper.Map<Order>(model);
            order.DateTime = DateTime.Now;
            order.Type = Enum.Parse<OrderType>(model.OrderType);
            order.EmployeeId = employee.Id;
            var items = this.context
                .Items
                .FirstOrDefault(i => i.Name == model.ItemName);

            order.OrderItems.Add(new OrderItem()
            {
                ItemId = items.Id,
                Order = order,
                Quantity = model.Quantity
            }); 


            this.context.Add(order);
            this.context.SaveChanges();

            return this.RedirectToAction("All", "Orders");
        }

        public IActionResult All()
        {
            var orders = this.context
                .Orders
                .ProjectTo<OrderAllViewModel>(mapper.ConfigurationProvider)
                .ToList();

            return this.View(orders);
        }
    }
}
