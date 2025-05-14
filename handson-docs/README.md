## アプリケーションの実行手順

### PowerShellで実行する場合

1. PowerShellを開き、プロジェクトのルートディレクトリに移動します
1. NuGetパッケージを復元します
   ```powershell
   dotnet restore
   ```
1. アプリケーションをビルドします
   ```powershell
   dotnet build
   ```
1. アプリケーションを実行します
   ```powershell
   dotnet run
   ```

### Visual Studio 2022で実行する場合

1. Visual Studio 2022でソリューションファイル（.sln）を開きます
1. スタートアッププロジェクトを右クリックし、「スタートアッププロジェクトに設定」を選択します
1. ツールバーの「デバッグの開始」ボタン（またはF5キー）をクリックしてアプリケーションを実行します

## データ追加用のPowerShellスクリプト

```powershell
# API エンドポイント URL
$Url = "https://localhost:7514/customers"

# 顧客データのリスト
$Customers = @(
    @{
        Name  = "アリス ジョンソン"
        Email = "alice.johnson@example.com"
        Phone = "111-222-3333"
    },
    @{
        Name  = "ボブ スミス"
        Email = "bob.smith@example.com"
        Phone = "222-333-4444"
    },
    @{
        Name  = "チャーリー ブラウン"
        Email = "charlie.brown@example.com"
        Phone = "333-444-5555"
    },
    @{
        Name  = "ダイアナ プリンス"
        Email = "diana.prince@example.com"
        Phone = "444-555-6666"
    },
    @{
        Name  = "イーサン ハント"
        Email = "ethan.hunt@example.com"
        Phone = "555-666-7777"
    }
)

# HTTP リクエストのヘッダー
$Headers = @{
    "Content-Type" = "application/json; charset=utf-8"
}

# 各顧客データを POST リクエストで送信
foreach ($Customer in $Customers) {
    # JSON に変換
    $Payload = $Customer | ConvertTo-Json -Depth 10
    # UTF-8バイト配列に変換
    $Bytes = [System.Text.Encoding]::UTF8.GetBytes($Payload)

    # POST リクエストの実行
    try {
        $Response = Invoke-RestMethod -Uri $Url -Method Post -Headers $Headers -Body $Bytes
        Write-Host "Success: " ($Response | ConvertTo-Json -Depth 10)
    } catch {
        Write-Host "Error: " $_.Exception.Message
    }
}
```