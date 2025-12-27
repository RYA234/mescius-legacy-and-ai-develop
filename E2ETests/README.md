# E2Eテスト - TextBoxControlTest

## テスト内容

**書式による入力制限 > テキストコントロール** でテキストに「2212」を入力し、スクリーンショットを撮影するテスト

## 実行方法

### 1. InputManアプリケーションをビルド

```bash
cd InputMan
msbuild /p:Configuration=Debug /p:Platform=x86
```

### 2. テストを実行

Visual Studio Test Explorerで実行するか、コマンドラインから:

```bash
cd E2ETests
dotnet test --logger "console;verbosity=detailed"
```

または、特定のテストのみ実行:

```bash
dotnet test --filter "FullyQualifiedName~TextBoxControlTest.TestTextBoxInput"
```

## スクリーンショット

テスト実行後、以下のフォルダにスクリーンショットが保存されます:

```
E2ETests\bin\Debug\net48\Screenshots\
```

保存されるスクリーンショット:
- `{timestamp}_01_TextControlPage_Loaded.png` - ページ読み込み後
- `{timestamp}_02_Before_Input.png` - 入力前
- `{timestamp}_03_After_Input.png` - 入力後
- `{timestamp}_Error_{time}.png` - エラー発生時（エラー時のみ）

## 要素が見つからない場合のトラブルシューティング

このテストコードには、要素が見つからない問題に対処する機能が含まれています:

### 1. 複数の検索方法を試行

```vb
' 方法1: AutomationIdで検索
textBox = mainWindow.Get(Of TextBox)(SearchCriteria.ByAutomationId("gcTextBox1"))

' 方法2: すべてのTextBoxを列挙して座標から推測
If textBox Is Nothing Then
    ' 座標が(100, 100)より小さい位置にあるTextBoxを探す
End If
```

### 2. デバッグ情報の出力

テスト実行時、コンソールに以下の情報が出力されます:
- 見つかったTextBoxの数
- 各TextBoxのId, Name, Location
- ツリーノードの一覧

### 3. 全コントロールダンプ

要素が見つからない場合、自動的にすべてのコントロール情報をダンプします:

```
===== すべてのコントロールをダンプ =====
[0] Type=Button, Id=closeButton, Name=Close, Location=(100,50)
[1] Type=TextBox, Id=gcTextBox1, Name=, Location=(8,29)
...
```

## コードの特徴

### 試用版ダイアログの自動処理

アプリ起動時に表示される「ライセンスについて」ダイアログを自動的に閉じます:

```vb
Private Shared Sub CloseTrialDialog()
    ' すべてのウィンドウを探索
    For Each window In app.GetWindows()
        ' 「ライセンス」または「トライアル」を含むタイトルを検索
        If window.Title.Contains("ライセンス") OrElse window.Title.Contains("トライアル") Then
            ' 方法1: OKボタンをクリック
            Dim okButton = window.Get(Of Button)(SearchCriteria.ByText("OK"))
            okButton.Click()

            ' 方法2（フォールバック）: Enterキーを送信
            WinForms.SendKeys.SendWait("{ENTER}")
        End If
    Next
End Sub
```

この処理により、テストが自動実行可能になります。

### 要素検索の堅牢性

1. **AutomationIdによる検索（第1優先）**
   ```vb
   textBox = mainWindow.Get(Of TextBox)(SearchCriteria.ByAutomationId("gcTextBox1"))
   ```

2. **座標による推測（フォールバック）**
   ```vb
   ' Designer.vbの情報: gcTextBox1は Location = (8, 29)
   If tb.Location.X < 100 AndAlso tb.Location.Y < 100 Then
       textBox = tb
   End If
   ```

3. **SendKeysによる入力（フォールバック）**
   ```vb
   ' TextBoxにキャストできない場合
   System.Windows.Forms.SendKeys.SendWait("^a")  ' Ctrl+A
   System.Windows.Forms.SendKeys.SendWait("2212")
   ```

### 待機処理

適切な待機時間を設定:
- アプリ起動: 3秒
- ページ遷移: 1秒
- 入力前後: 300-500ミリ秒

### エラーハンドリング

- エラー時のスクリーンショット自動撮影
- 詳細なログ出力
- スタックトレースの表示

## 座標情報の使用について

このテストでは、**AutomationIdが最優先**ですが、見つからない場合の**フォールバック**として座標情報を使用しています:

```vb
' Designer.vbから取得した座標情報
' gcTextBox1: Location = (8, 29)
' この情報を使って、左上付近のTextBoxを探す
If tb.Location.X < 100 AndAlso tb.Location.Y < 100 Then
    textBox = tb
End If
```

これは**絶対座標指定ではなく、範囲による推測**なので、ある程度柔軟性があります。

## 既知の問題

### 問題1: ツリーノードの展開が遅い

**症状**: ツリーノードをクリックしてもページが表示されない

**対処**: `Thread.Sleep`の時間を増やす
```vb
Thread.Sleep(1000) ' 1秒 → 2秒に変更
```

### 問題2: TextBoxが見つからない

**症状**: "テキストボックスが見つかりませんでした" エラー

**対処**:
1. コンソール出力を確認して、実際のAutomationIdを確認
2. Inspect.exeツール（Windows SDK）で実際のAutomationIdを調査
3. 座標範囲を調整（現在: X<100, Y<100）

### 問題3: 入力した値が消える

**症状**: 入力後に値が空になる

**対処**: フォーカスイベントやバリデーションが原因の可能性
```vb
textBox.Focus()
Thread.Sleep(500) ' フォーカス待機を増やす
textBox.Text = "2212"
```

## 参考資料

- TestStack.White ドキュメント: https://github.com/TestStack/White
- UI Automation API: https://docs.microsoft.com/en-us/windows/win32/winauto/entry-uiauto-win32
- Inspect.exe (Windows SDK ツール): 要素のプロパティを調査
