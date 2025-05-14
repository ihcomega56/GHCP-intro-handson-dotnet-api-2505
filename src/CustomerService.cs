using GHCP_intro_handson_2505_dotnet_api.Models;
using GHCP_intro_handson_2505_dotnet_api.Data;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models.Dto;

namespace GHCP_intro_handson_2505_dotnet_api.Services
{
    public class CustomerService
    {
        private readonly CustomerDbContext _db;

        public CustomerService(CustomerDbContext db)
        {
            _db = db;
        }

        // 取引先一覧を返す
        public async Task<List<CustomerDto>> GetCustomerListAsync()
        {
            // 取引先一覧を取得
            var result = await _db.Customers.ToListAsync();
            var data = new List<CustomerDto>();
            foreach (var c in result)
            {
                // 各顧客のトランザクション数を取得
                var cnt = await _db.Transactions.CountAsync(t => t.CustomerId == c.Id);

                data.Add(new CustomerDto
                {
                    Customer = new Customer
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email,
                        Phone = c.Phone
                    },
                    TransactionCount = cnt,
                    ResultMessage = null
                });
            }
            return data;
        }

        // 取引先詳細を返す
        public async Task<Customer> GetCustomerDetailAsync(int? custId = null)
        {
            if (!custId.HasValue)
            {
                throw new ArgumentNullException("顧客IDが指定されていません");
            }

            var c = await _db.Customers.FirstOrDefaultAsync(c => c.Id == custId.Value);

            if (c == null)
            {
                throw new KeyNotFoundException("指定されたIDの取引先が見つかりません");
            }

            // 1件の取引先の詳細を返す
            return new Customer
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            };
        }

        public async Task<CustomerDto> CreateCustomerAsync(Customer? custData = null)
        {
            if (custData == null)
            {
                throw new ArgumentNullException("顧客データが指定されていません");
            }

            // 新規取引先を作成
            _db.Customers.Add(custData);
            await _db.SaveChangesAsync();

            return new CustomerDto
            {
                Customer = new Customer
                {
                    Id = custData.Id,
                    Name = custData.Name,
                    Email = custData.Email,
                    Phone = custData.Phone
                },
                ResultMessage = "取引先を登録しました"
            };
        }
         
          // 取引先に紐づく取引の操作を行うためのメソッド
        public async Task<object> ProcessTransactionOperationAsync(string operation, int? custId = null, Customer? custData = null, Transaction? trData = null)
        {
            if (operation == "transactions")
            {
                if (!custId.HasValue)
                {
                    throw new ArgumentNullException("顧客IDが指定されていません");
                }
                
                var c = _db.Customers.FirstOrDefault(c => c.Id == custId.Value);
                
                if (c == null)
                {
                    throw new KeyNotFoundException("指定されたIDの取引先が見つかりません");
                }
                
                // 特定の取引先の取引履歴を取得（最新20件）
                var txs = _db.Transactions
                    .Where(t => t.CustomerId == custId.Value)
                    .OrderByDescending(t => t.Date)
                    .Take(20)
                    .ToList();
                
                var d = new List<object>();
                foreach (var t in txs)
                {
                    d.Add(new 
                    {
                        id = t.Id,
                        date = t.Date.ToString("yyyy-MM-dd"),
                        amount = t.Amount,
                        memo = t.Memo
                    });
                }
                
                return new
                {
                    customerId = custId.Value,
                    customerName = c.Name,
                    transactions = d
                };
            }
            else if (operation == "add-transaction")
            {
                if (trData == null)
                {
                    throw new ArgumentNullException("取引データが指定されていません");
                }
                
                // 取引先の存在確認
                var c = _db.Customers.FirstOrDefault(c => c.Id == trData.CustomerId);
                
                if (c == null)
                {
                    throw new KeyNotFoundException("指定されたIDの取引先が見つかりません");
                }
                
                // 新規取引を登録
                _db.Transactions.Add(trData);
                await _db.SaveChangesAsync();
                
                return new
                {
                    id = trData.Id,
                    customerId = trData.CustomerId,
                    customerName = c.Name,
                    date = trData.Date.ToString("yyyy-MM-dd"),
                    amount = trData.Amount,
                    memo = trData.Memo,
                    message = "取引を登録しました"
                };
            }
            else
            {
                throw new NotSupportedException($"操作 '{operation}' はサポートされていません");
            }
        }
    }
}
