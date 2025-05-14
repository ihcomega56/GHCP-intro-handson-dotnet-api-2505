## アプリケーションのダウンロード

### Gitを使う場合

※IDEに付属のGit機能をお使いいただいてもかまいません

#### HTTPS認証とPersonal Access Tokenを使う場合

次のコマンドを実行します

```shell
cd /path/to/your-workspace-folder/
git clone https://github.com/ihcomega56/GHCP-intro-handson-dotnet-api-2505.git
```

#### SSH認証を使う場合

次のコマンドを実行します

```shell
cd /path/to/your-workspace-folder/
git clone git@github.com:ihcomega56/GHCP-intro-handson-dotnet-api-2505.git
```

### Gitを使わない場合

1. https://github.com/ihcomega56/GHCP-intro-handson-dotnet-api-2505 にアクセスします
1. 画面上部右側の `Code` ボタン（緑色） -> `Download ZIP` ボタンの順にクリックします

## アプリケーションの実行手順

### Visual Studio 2022で実行する場合 

Visual Studio のデバッグ機能を体験するにはこちらがおすすめです。

1. Visual Studio 2022でソリューションファイル `CusomerApi.sln` を開きます
1. プロジェクト `CustomerApi.csproj` を右クリックし、「スタートアッププロジェクトに設定」を選択します
1. ツールバーの「デバッグの開始」ボタン（またはF5キー）をクリックしてアプリケーションを実行します

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

## データ登録用のPowerShellスクリプト

開発環境ではアプリケーションを立ち上げるとシードデータが入力されるようになっています。追加でデータを登録したい場合は次のスクリプトをお使いください

### 顧客データ登録

```powershell
# API エンドポイント URL
$Url = "http://localhost:5514/customers"

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