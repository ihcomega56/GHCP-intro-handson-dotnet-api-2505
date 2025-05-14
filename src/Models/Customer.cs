using System.Text.Json.Serialization;

namespace GHCP_intro_handson_2505_dotnet_api.Models
{
    /// <summary>
    /// 取引先を表すモデル
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 顧客ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 顧客名
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// メールアドレス
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// 電話番号
        /// </summary>
        public required string Phone { get; set; }
        /// <summary>
        /// 取引履歴のナビゲーションプロパティ
        /// </summary>
        [JsonIgnore]
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
