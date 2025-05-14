using GHCP_intro_handson_2505_dotnet_api.Models;

namespace YourNamespace.Models.Dto
{
    /// <summary>
    /// 顧客登録後のAPI戻り値用DTO
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// 登録された顧客情報
        /// </summary>
        public required Customer Customer { get; set; }

        /// <summary>
        /// 取引数
        /// </summary>
        public int? TransactionCount { get; set; }

        /// <summary>
        /// 結果を格納するメッセージ
        /// </summary>
        public string? ResultMessage { get; set; }
    }
}
