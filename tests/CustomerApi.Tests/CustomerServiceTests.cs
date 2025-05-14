using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GHCP_intro_handson_2505_dotnet_api.Models;
using GHCP_intro_handson_2505_dotnet_api.Services;
using GHCP_intro_handson_2505_dotnet_api.Data;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models.Dto;

namespace GHCP_intro_handson_2505_dotnet_api.Services.Tests
{
    [TestClass]
    public class CustomerServiceTests
    {
        private CustomerDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new CustomerDbContext(options);
        }

        private void SeedTestData(CustomerDbContext db)
        {
            var customers = new List<Customer>
                    {
                        new Customer { Id = 1, Name = "Alice", Email = "alice@example.com", Phone = "123-4567" },
                        new Customer { Id = 2, Name = "Bob", Email = "bob@example.com", Phone = "987-6543" }
                    };
            db.Customers.AddRange(customers);

            var transactions = new List<Transaction>
                    {
                        new Transaction { Id = 1, CustomerId = 1, Date = DateTime.Today, Amount = 100, Memo = "Test1" },
                        new Transaction { Id = 2, CustomerId = 1, Date = DateTime.Today.AddDays(-1), Amount = 200, Memo = "Test2" },
                        new Transaction { Id = 3, CustomerId = 2, Date = DateTime.Today, Amount = 300, Memo = "Test3" }
                    };
            db.Transactions.AddRange(transactions);
            db.SaveChanges();
        }

        [TestMethod]
        public async Task GetCustomerListAsync_List_ReturnsAllCustomers()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            SeedTestData(db);
            var service = new CustomerService(db);

            // Act
            var result = await service.GetCustomerListAsync();
            var customers = result as List<CustomerDto>;

            // Assert
            Assert.IsNotNull(customers);
            Assert.AreEqual(2, customers.Count);
            // 動的オブジェクトのプロパティをチェック
            Assert.IsTrue(customers.Any(c => c.Customer.Name == "Alice"));
            Assert.IsTrue(customers.Any(c => c.Customer.Name == "Bob"));
        }

        [TestMethod]
        public async Task GetCustomerDetailAsync_Get_ReturnsCorrectCustomer()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            SeedTestData(db);
            var service = new CustomerService(db);

            // Act
            var result = await service.GetCustomerDetailAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Alice", result.Name);
            Assert.AreEqual("alice@example.com", result.Email);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetCustomerDetailAsync_Get_ThrowsForNonExistent()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            SeedTestData(db);
            var service = new CustomerService(db);

            // Act - Should throw KeyNotFoundException
            await service.GetCustomerDetailAsync(999);
        }

        [TestMethod]
        public async Task CreateCustomerAsync_Create_AddsCustomerSuccessfully()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var service = new CustomerService(db);
            var newCustomer = new Customer { Name = "Charlie", Email = "charlie@example.com", Phone = "555-0000" };

            // Act
            var result = await service.CreateCustomerAsync(custData: newCustomer);
            var customers = db.Customers.ToList();

            // Assert
            Assert.AreEqual(1, customers.Count);
            Assert.AreEqual("Charlie", customers.First().Name);

            // 結果オブジェクトの検証
            Assert.IsNotNull(result.Customer);
            Assert.AreEqual("Charlie", result.Customer.Name);
        }
        
        [TestMethod]
        public async Task ProcessCustomerOperation_Transactions_ReturnsCorrectTransactions()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            SeedTestData(db);
            var service = new CustomerService(db);

            // Act
            var result = await service.ProcessTransactionOperationAsync("transactions", 1);

            // Assert
            var properties = result.GetType().GetProperties();
            var transactionsProperty = properties.FirstOrDefault(p => p.Name == "transactions");
            Assert.IsNotNull(transactionsProperty);

            var transactions = transactionsProperty.GetValue(result) as List<object>;
            Assert.IsNotNull(transactions);
            Assert.AreEqual(2, transactions.Count);
        }

        [TestMethod]
        public async Task ProcessCustomerOperation_AddTransaction_AddsTransactionSuccessfully()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            SeedTestData(db);
            var service = new CustomerService(db);
            var newTransaction = new Transaction
            {
                CustomerId = 1,
                Date = DateTime.Today,
                Amount = 500,
                Memo = "Test New"
            };

            // Act
            var result = await service.ProcessTransactionOperationAsync("add-transaction", trData: newTransaction);
            var transactions = db.Transactions.Where(t => t.CustomerId == 1).ToList();

            // Assert
            Assert.AreEqual(3, transactions.Count);
            Assert.IsTrue(transactions.Any(t => t.Amount == 500 && t.Memo == "Test New"));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task ProcessCustomerOperation_InvalidOperation_ThrowsException()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var service = new CustomerService(db);

            // Act & Assert - Should throw NotSupportedException
            await service.ProcessTransactionOperationAsync("invalid-operation");
        }}
}