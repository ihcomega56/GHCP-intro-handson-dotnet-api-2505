namespace GHCP_intro_handson_2505_dotnet_api.Models
{
    /// <summary>
    /// 取引履歴を表すモデル
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// 取引ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 取引日
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 金額
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; } = string.Empty;

        /// <summary>
        /// 顧客ID（外部キー）
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// 取引先ナビゲーションプロパティ
        /// </summary>
        public Customer? Customer { get; set; }
    }
}
