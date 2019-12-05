using Microsoft.EntityFrameworkCore;

namespace FastFood.Data
{
    using Models;

    public class FastFoodDbContext : DbContext
	{
		public FastFoodDbContext()
		{
		}

		public FastFoodDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			if (!builder.IsConfigured)
			{
				builder.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Position>()
                .HasIndex(p => p.Name)
                .IsUnique();

            builder.Entity<Item>()
                .HasIndex(i => i.Name)
                .IsUnique();

            builder.Entity<OrderItem>()
                .HasKey(oi => new {oi.ItemId, oi.OrderId});
        }
	}
}