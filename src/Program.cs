using Microsoft.EntityFrameworkCore;
using GHCP_intro_handson_2505_dotnet_api.Models;
using GHCP_intro_handson_2505_dotnet_api.Data;
using GHCP_intro_handson_2505_dotnet_api.Services;

var builder = WebApplication.CreateBuilder(args);

// DBコンテキストの登録
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseInMemoryDatabase("CustomerDb"));

// サービスの登録
builder.Services.AddScoped<CustomerService>();

var app = builder.Build();

app.UseHttpsRedirection();

// 取引先一覧を取得するエンドポイント
app.MapGet("/customers", async (CustomerService service) =>
{
    try
    {
        var customers = await service.GetCustomerListAsync();
        return Results.Ok(customers.Select(c => new
        {
            customer = c.Customer,
            transactionCount = c.TransactionCount
        }).ToList());
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetCustomers")
.WithDescription("取引先一覧を取得します");

// 取引先詳細を取得するエンドポイント
app.MapGet("/customers/{id}", async (int id, CustomerService service) =>
{
    try
    {
        var customer = await service.GetCustomerDetailAsync(id);
        return Results.Ok(new
        {
            customer = customer.Customer,
            transactionCount = customer.TransactionCount
        }
        );
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetCustomerById")
.WithDescription("指定したIDの取引先詳細を取得します");

// 新規取引先を登録するエンドポイント
app.MapPost("/customers", async (Customer customer, CustomerService service) =>
{
    try
    {
        var result = await service.CreateCustomerAsync(custData: customer);
        return Results.Created($"/customers/{result.Customer?.Id}", result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateCustomer")
.WithDescription("新規取引先を登録します");

app.MapGet("/customers/{id}/transactions", async (int id, CustomerService service) =>
{
    try
    {
        var transactions = await Task.FromResult(service.ProcessTransactionOperationAsync("transactions", id).Result);
        return Results.Ok(transactions);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetCustomerTransactions")
.WithDescription("指定した取引先の最新20件の取引履歴を取得します");

app.MapPost("/transactions", async (Transaction transaction, CustomerService service) =>
{
    try
    {
        var result = await service.ProcessTransactionOperationAsync("add-transaction", trData: transaction);
        return Results.Created($"/transactions/{((dynamic)result).id}", result);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateTransaction")
.WithDescription("新規取引を登録します");

// 初期データ投入（開発環境のみ）
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    // シードデータを投入
    dbContext.Customers.AddRange(
        new Customer { Name = "田中 太郎", Email = "tanaka@example.com", Phone = "090-1111-2222" },
        new Customer { Name = "佐藤 花子", Email = "sato@example.com", Phone = "080-3333-4444" },
        new Customer { Name = "鈴木 次郎", Email = "suzuki@example.com", Phone = "070-5555-6666" }
    );
    dbContext.Transactions.AddRange(
        new Transaction { CustomerId = 1, Date = DateTime.Today, Amount = 1000, Memo = "初回取引" },
        new Transaction { CustomerId = 1, Date = DateTime.Today.AddDays(-1), Amount = 2000, Memo = "2回目の取引" },
        new Transaction { CustomerId = 2, Date = DateTime.Today.AddDays(-2), Amount = 1500, Memo = "初回取引" },
        new Transaction { CustomerId = 2, Date = DateTime.Today.AddDays(-3), Amount = 2500, Memo = "2回目の取引" },
        new Transaction { CustomerId = 2, Date = DateTime.Today.AddDays(-4), Amount = 3000, Memo = "3回目の取引" }
    );
    dbContext.SaveChanges();
}

app.Run();
