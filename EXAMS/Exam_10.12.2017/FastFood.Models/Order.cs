namespace FastFood.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Enums;

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string Customer { get; set; }

        public DateTime DateTime { get; set; }

        
        public OrderType Type { get; set; } = OrderType.ForHere;

        [NotMapped] 
        public decimal TotalPrice => this.OrderItems.Sum(oi => oi.Item.Price * (decimal)oi.Quantity);

        public int EmployeeId { get; set; }

        [Required]
        public Employee Employee { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }
}