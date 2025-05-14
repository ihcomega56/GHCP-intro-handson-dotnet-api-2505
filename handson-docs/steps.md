ハンズオン手順です。Visual Studio 2022でソースコードを開いた状態で始めます。独力で進められる方は講師を待たずに次の手順へ進んで構いません。

## プロジェクトの分析

### プロジェクト全体を把握する

1. Copilot Chat でプロジェクトについて質問してみましょう。 `#` でコンテキストの付与ができます
    - `#solution 説明してください`
    - `/explain #file:'CustomerApi.csproj'`
    - `#file:'Program.cs' からこのアプリケーションの主な動きを読み解いてください。`
    - `@workspace モデルクラスはどこにありますか？`
    - その他、皆さんが気になることを何でも質問してみてください。

### ドキュメントを出力し、理解に役立てる

1. Copilot Chat で OpenAPI ドキュメントを出力してみましょう
    - `#file:'Program.cs' からOpenAPIドキュメントを出力してください。`
1. 出力したドキュメントを https://editor.swagger.io/ で確認します
1. Copilot Chat で Mermaid記法を使ったドキュメントを出力してみましょう
    - `#class:'GHCP_intro_handson_2505_dotnet_api.Models.Customer':159-817 と #class:'GHCP_intro_handson_2505_dotnet_api.Models.Transaction':119-847 を Mermaid記法でクラス図にしてください。` ※クラス名は `#` の後に名前の一部を入力すると補完されます
1. 出力したドキュメントを https://mermaid.live/ で確認します

## コメントの付与

1. `Program.cs` でコード補完を使ってエンドポイントについてコメントで補足しましょう
    - 次の定義の上の行で `//` と入力すると、Copilot による提案が表示されます（されない場合もあります）
        - `GET /customers/{id}/transactions`
        - `POST /transactions`
1. `CustomerService.cs` で Copilot Chat を使ってドキュメントコメントを書きましょう
    - `CustomerService.cs` を開きます（ `#` とは異なるコンテキストの渡し方です）
    - `ドキュメントコメントを書いてください` ※ここではファイル全体を取得したいので、レスポンス内で記載の省略があったら改めてリクエストしてください。例： `ドキュメントコメントを書いたファイル全体を見せてください`
    - `プレビュー` ボタンでソースコードに反映し、差分を確認したのち取り込みます

## リファクタリング

### N+1問題の解消

リファクタリングの例として、N+1問題の解消をしてみましょう。課題を見つけるところから実行します

1. `CustomerService.cs` を開きます
1. `/optimize` コマンドを入力します。おそらくN+1について指摘されるはずです ※VSの補完によりコマンドが無効になる場合があります。その際は余分な指示を削って `/optimize` のみ送信してください
1. インラインチャットで Copilot に修正を依頼します。 `Alt + /` 
1. Copilot Chatで改修内容について質問してみましょう
    - `他にも修正方法はありますか？`
    - `Includeを使う場合とSelectを使う場合とでどのような違いがありますか？`
    - `N+1問題とは何ですか？`
    - 前に入力したプロンプトをもとに新たなプロンプトが提案されることもあります。クリックで入力してみましょう

#### 改修例

```csharp
public async Task<List<CustomerDto>> GetCustomerListAsync()
{
    // 顧客とトランザクション数を一括で取得し、N+1問題を回避
    var customerList = await _db.Customers
        .Select(c => new
        {
            Customer = c,
            TransactionCount = _db.Transactions.Count(t => t.CustomerId == c.Id)
        })
        .ToListAsync();

    var data = new List<CustomerDto>(customerList.Count);
    foreach (var item in customerList)
    {
        data.Add(new CustomerDto
        {
            Customer = new Customer
            {
                Id = item.Customer.Id,
                Name = item.Customer.Name,
                Email = item.Customer.Email,
                Phone = item.Customer.Phone
            },
            TransactionCount = item.TransactionCount,
            ResultMessage = null
        });
    }
    return data;
}
```

```csharp
public async Task<List<CustomerDto>> GetCustomerListAsync()
{
    // Customers と Transactions を一括取得し、N+1問題を解消
    var customers = await _db.Customers
        .Include(c => c.Transactions)
        .ToListAsync();

    var data = new List<CustomerDto>();
\    foreach (var c in customers)
     {
        var cnt = c.Transactions?.Count ?? 0;

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
```

### ワーク：リファクタリング

`CustomerService.cs`の`ProcessTransactionOperationAsync(string operation, int? custId = null, Customer? custData = null, Transaction? trData = null)`およびその呼出箇所はリファクタリングの余地が多くあります。Copilotを使いながら修正してみましょう

改善活動の例：

- 該当メソッドの問題を Copilot Chat に尋ねます
- Mermaid記法でシーケンス図を書いて課題を抽出します
- Copilot Edits で方針を与えながら実際に修正を実行します
- 単体テストを追加し、実行しながら改善することでデグレを防ぎます

#### 単体テスト追加

Copilotと相談しながらテストを追加しましょう。すでにいくつかのテストケースを `CustomerServiceTests.cs` に作成済みです

作業例：

- Copilot Chatに尋ねます
    - `#ProcessTransactionOperationAsync にはどんな単体テストが必要ですか？`
    - `現在どのようなテストがすでに存在しますか？`
- コード補完を活用して書きます

```csharp
// 例として、顧客の取引履歴取得時、存在しない顧客IDを渡した場合のテストを追加します
[TestMethod]
[ExpectedException(typeof(KeyNotFoundException))]
public async Task ProcessCustomerOperation_GetTransactions_ThrowsException()
{
    // 中身はおそらくコード補完が働きます
}
```
#### 非同期処理の改善

Copilotと相談しながら適切に非同期処理が行えていない箇所を修正しましょう

作業例：

- Copilot Chatに尋ねます
    - `@workspace 非同期処理に問題のある箇所を洗い出してください`
    - `なぜこの処理に問題があるのですか？`
    - `C#の非同期処理について教えてください`
- インラインチャット、Copilot Chat、Copilot Editsを使って修正します


#### メソッド分割

Copilotと相談しながら `ProcessTransactionOperationAsync` を2つのメソッドに分けましょう