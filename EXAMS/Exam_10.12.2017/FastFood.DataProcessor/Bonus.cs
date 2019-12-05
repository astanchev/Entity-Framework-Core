using System;
using FastFood.Data;

namespace FastFood.DataProcessor
{
    using System.Linq;

    public static class Bonus
    {
	    public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
        {
            var itemToBeUpdated = context
                .Items
                .FirstOrDefault(i => i.Name == itemName);

            if (itemToBeUpdated == null)
            {
                return $"Item {itemName} not found!";
            }

            var oldPrice = itemToBeUpdated.Price;

            itemToBeUpdated.Price = newPrice;

            context.Items.Update(itemToBeUpdated);
            context.SaveChanges();

            return $"{itemToBeUpdated.Name} Price updated from ${oldPrice:F2} to ${newPrice:F2}";
        }
    }
}
