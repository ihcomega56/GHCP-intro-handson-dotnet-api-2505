using Microsoft.EntityFrameworkCore;
using GHCP_intro_handson_2505_dotnet_api.Models;

namespace GHCP_intro_handson_2505_dotnet_api.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        // モデルの構成
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 取引先と取引の関係を定義
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CustomerId);
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
