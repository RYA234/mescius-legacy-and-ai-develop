# InputMan E2Eテスト作成ガイド（TestStack.White）

## 概要

このガイドは、**InputMan for Windows Forms 12.0J**アプリケーションのE2Eテスト作成方法を説明します。TestStack.Whiteを使用したUI自動化テストの実装パターンとベストプラクティスをまとめています。

**対象アプリケーション**: `InputMan/InputManWin12_Demo.exe`

## フレームワーク選定理由

本プロジェクトでTestStack.Whiteを採用した理由は、**対象アプリケーションが.NET Framework 4.8で動作するレガシーシステム**だからです。

### TestStack.Whiteの特徴

- **.NET Framework専用**: .NET Framework 4.x環境で安定動作
- **Windows FormsとWPFに対応**: UI Automation APIベースの堅牢な要素検索
- **枯れた技術**: 長年の実績があり、トラブルシューティング情報が豊富

### .NET移行時の注意事項

**重要**: 将来的に.NET（.NET 6/7/8以降）に移行する場合、TestStack.Whiteは使用できません。以下の代替フレームワークへの移行が必要です：

#### 推奨代替フレームワーク

1. **Appium WinAppDriver** (推奨)
   - クロスプラットフォーム対応
   - WebDriver互換のAPI
   - .NET / .NET Frameworkどちらも対応
   - Appiumエコシステムの豊富なツール
   ```bash
   dotnet add package Appium.WebDriver
   ```

2. **FlaUI**
   - .NET/.NET Framework両対応
   - UI Automation APIの直接ラッパー
   - TestStack.Whiteの後継的な位置づけ
   ```bash
   dotnet add package FlaUI.Core
   dotnet add package FlaUI.UIA3  # または FlaUI.UIA2
   ```

3. **Playwright for .NET** (Webアプリの場合)
   - モダンなブラウザ自動化
   - Windows FormsからBlazor/Webへ移行する場合に最適
   ```bash
   dotnet add package Microsoft.Playwright
   ```

#### 移行時の考慮事項

- **API互換性なし**: コードの書き換えが必須
- **要素検索ロジック**: AutomationIdベースの検索ロジックは概ね同じ考え方
- **多層防御の考え方**: 本ガイドで示した戦略（AutomationId → 座標範囲 → ダンプ）は移行後も有効
- **テストアーキテクチャ**: Page Objectパターンなどを導入すれば移行コストを削減可能

## 前提条件

### 必要なパッケージ

```xml
<ItemGroup>
  <PackageReference Include="MSTest" Version="4.0.1" />
  <PackageReference Include="TestStack.White" Version="0.13.3" />
</ItemGroup>

<ItemGroup>
  <Reference Include="System.Drawing" />
  <Reference Include="System.Windows.Forms" />
  <Reference Include="WindowsBase" />
  <Reference Include="UIAutomationTypes" />
  <Reference Include="UIAutomationClient" />
</ItemGroup>
```

### 基本的なImports

```vb
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TestStack.White
Imports TestStack.White.UIItems
Imports TestStack.White.UIItems.Finders
Imports TestStack.White.UIItems.WindowItems
Imports TestStack.White.UIItems.TreeItems
Imports System.IO
Imports System.Threading
Imports Drawing = System.Drawing
Imports WinForms = System.Windows.Forms
```

## ソースコードからの情報収集（重要）

E2Eテスト作成では、**ソースコードへのアクセスが効率を大幅に向上させます**。

### 1. Designer.vbファイルからコントロール情報を取得

**確認すべき情報**:
- AutomationId（`Name` プロパティ）
- 座標（`Location` プロパティ）
- サイズ（`Size` プロパティ）

**例**: `GcTextBox.Designer.vb`
```vb
Me.gcTextBox1.Name = "gcTextBox1"                    ' AutomationId
Me.gcTextBox1.Location = New System.Drawing.Point(8, 29)  ' 座標
Me.gcTextBox1.Size = New System.Drawing.Size(260, 24)     ' サイズ
```

### 2. MainFormからアプリケーション構造を理解

**確認すべき情報**:
- ページ遷移ロジック
- ツリーメニューの構造
- コントロールの階層

**例**: `MainForm.vb`
```vb
Case "テキストコントロール"
    Me.setMainPanel(New InputManWin12_Demo._02_Format.GcTextBox())
```

## 要素検索の戦略（多層防御）

### 優先順位

1. **AutomationIdによる検索（最優先）**
2. **座標範囲による推測（フォールバック）**
3. **全コントロールダンプ（デバッグ）**

### 実装パターン

```vb
' 方法1: AutomationIdで検索
Dim textBox As UIItem = Nothing
Try
    textBox = mainWindow.Get(Of TextBox)(SearchCriteria.ByAutomationId("gcTextBox1"))
    Console.WriteLine("✓ AutomationIdで見つかりました")
Catch ex As Exception
    Console.WriteLine($"✗ AutomationId検索失敗: {ex.Message}")
End Try

' 方法2: 見つからない場合は座標範囲で推測
If textBox Is Nothing Then
    Console.WriteLine("すべてのTextBoxを検索中...")
    Dim allTextBoxes = mainWindow.GetMultiple(
        SearchCriteria.ByControlType(GetType(TextBox), WindowsFramework.Win32))

    For Each tb In allTextBoxes
        Dim loc = tb.Location
        Console.WriteLine($"TextBox: Id={tb.Id}, Location=({loc.X},{loc.Y})")

        ' Designer.vbの座標情報を活用: Location = (8, 29)
        ' 範囲で検索（左上付近 = X<100, Y<100）
        If loc.X < 100 AndAlso loc.Y < 100 Then
            textBox = tb
            Console.WriteLine("✓ 座標から推測成功")
            Exit For
        End If
    Next
End If

' 方法3: それでも見つからない場合は全ダンプ
If textBox Is Nothing Then
    DumpAllControls(mainWindow)
    Assert.Fail("テキストボックスが見つかりませんでした")
End If
```

## テスト構造のベストプラクティス

### ClassInitialize - アプリ起動処理

```vb
<ClassInitialize>
Public Shared Sub ClassSetup(context As TestContext)
    ' 1. InputManアプリケーションパスの構築
    Dim solutionDir = Path.GetFullPath(Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."))
    Dim appPath = Path.Combine(solutionDir, "InputMan", "bin", "x86", "Debug", "InputManWin12_Demo.exe")

    Assert.IsTrue(File.Exists(appPath), $"InputManが見つかりません: {appPath}")

    ' 2. アプリケーション起動
    app = Application.Launch(appPath)
    Thread.Sleep(3000) ' 起動待機

    ' 3. 試用版ダイアログ処理（必要に応じて）
    CloseTrialDialog()

    ' 4. メインウィンドウ取得
    mainWindow = app.GetWindows()(0)

    ' 5. ウィンドウを画面内に移動（画面外対策）
    EnsureWindowOnScreen()

    Console.WriteLine($"✓ アプリ起動成功: {mainWindow.Title}")
End Sub
```

### TestMethod - テスト本体

```vb
<TestMethod>
Public Sub TestControlInput()
    Try
        ' ステップごとにログ出力
        Console.WriteLine("===== テスト開始 =====")

        ' ステップ1: ナビゲーション
        Console.WriteLine("ステップ1: ページ遷移...")
        NavigateToPage("対象ページ名")
        Thread.Sleep(2000) ' ページ読み込み待機

        ' スクリーンショット1: 遷移後
        CaptureScreenshot("01_Page_Loaded")

        ' ステップ2: 要素検索
        Console.WriteLine("ステップ2: コントロール検索...")
        Dim control = FindControl()

        ' スクリーンショット2: 操作前
        CaptureScreenshot("02_Before_Action")

        ' ステップ3: 操作実行
        Console.WriteLine("ステップ3: 操作実行...")
        PerformAction(control)

        ' スクリーンショット3: 操作後
        CaptureScreenshot("03_After_Action")

        ' ステップ4: 検証
        Console.WriteLine("ステップ4: 検証...")
        Assert.AreEqual("期待値", control.Text)

        Console.WriteLine("===== テスト成功 =====")

    Catch ex As Exception
        CaptureScreenshot($"Error_{DateTime.Now:HHmmss}")
        Console.WriteLine($"テストエラー: {ex.Message}")
        Throw
    End Try
End Sub
```

## 便利なヘルパーメソッド

### スクリーンショット撮影

```vb
Private Sub CaptureScreenshot(fileName As String)
    Try
        Dim dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots")
        Directory.CreateDirectory(dir)

        Dim timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss")
        Dim path = Path.Combine(dir, $"{timestamp}_{fileName}.png")

        ' 画面全体をキャプチャ
        Dim bounds = WinForms.Screen.PrimaryScreen.Bounds
        Dim bitmap As New Drawing.Bitmap(bounds.Width, bounds.Height)
        Using g = Drawing.Graphics.FromImage(bitmap)
            g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size)
        End Using
        bitmap.Save(path, Drawing.Imaging.ImageFormat.Png)

        Console.WriteLine($"✓ スクリーンショット: {path}")
    Catch ex As Exception
        Console.WriteLine($"✗ スクリーンショット失敗: {ex.Message}")
    End Try
End Sub
```

### 全コントロールダンプ（デバッグ用）

```vb
Private Sub DumpAllControls(window As Window)
    Console.WriteLine("===== コントロールダンプ =====")
    Try
        Dim controls = window.Items
        Console.WriteLine($"総数: {controls.Count}")

        For i = 0 To Math.Min(controls.Count - 1, 50)
            Dim ctrl = controls(i)
            Dim loc = ctrl.Location
            Console.WriteLine(
                $"[{i}] Type={ctrl.GetType().Name}, " &
                $"Id={ctrl.Id}, Name={ctrl.Name}, " &
                $"Location=({loc.X},{loc.Y})")
        Next
    Catch ex As Exception
        Console.WriteLine($"ダンプエラー: {ex.Message}")
    End Try
    Console.WriteLine("==============================")
End Sub
```

### ツリーノード操作

```vb
Private Sub ExpandTreeNode(nodeName As String)
    Dim trees = mainWindow.GetMultiple(
        SearchCriteria.ByControlType(GetType(Tree), WindowsFramework.Win32))

    If trees.Count > 0 Then
        Dim tree = TryCast(trees(0), Tree)
        Dim node = tree.Nodes.FirstOrDefault(Function(n) n.Text.Contains(nodeName))

        If node IsNot Nothing AndAlso Not node.IsExpanded Then
            node.Expand()
            Console.WriteLine($"✓ ノード展開: {nodeName}")
        End If
    End If
End Sub

Private Sub ClickTreeNode(nodeName As String)
    Dim trees = mainWindow.GetMultiple(
        SearchCriteria.ByControlType(GetType(Tree), WindowsFramework.Win32))

    If trees.Count > 0 Then
        Dim tree = TryCast(trees(0), Tree)

        ' 親ノードと子ノードを検索
        For Each parentNode In tree.Nodes
            If parentNode.Text.Contains(nodeName) Then
                parentNode.Click()
                Console.WriteLine($"✓ ノードクリック: {nodeName}")
                Return
            End If

            For Each childNode In parentNode.Nodes
                If childNode.Text.Contains(nodeName) Then
                    childNode.Click()
                    Console.WriteLine($"✓ ノードクリック: {nodeName}")
                    Return
                End If
            Next
        Next
    End If
End Sub
```

### 試用版ダイアログ処理

```vb
Private Shared Sub CloseTrialDialog()
    Try
        Console.WriteLine("試用版ダイアログ確認中...")
        Thread.Sleep(1500)

        For Each window In app.GetWindows()
            If window.Title.Contains("ライセンス") OrElse
               window.Title.Contains("トライアル") Then
                Console.WriteLine($"✓ ダイアログ発見: {window.Title}")

                ' 方法1: AutomationIdでOKボタンを探す
                Try
                    Dim okBtn = window.Get(Of Button)(
                        SearchCriteria.ByAutomationId("okButton"))
                    okBtn.Click()
                    Console.WriteLine("✓ OKボタンクリック")
                    Thread.Sleep(1500)
                    Return
                Catch
                End Try

                ' 方法2: テキストでOKボタンを探す
                Try
                    Dim okBtn = window.Get(Of Button)(
                        SearchCriteria.ByText("OK"))
                    okBtn.Click()
                    Console.WriteLine("✓ OKボタンクリック")
                    Thread.Sleep(1500)
                    Return
                Catch
                End Try

                ' 方法3: Enterキー送信
                Try
                    window.Focus()
                    WinForms.SendKeys.SendWait("{ENTER}")
                    Console.WriteLine("✓ Enter送信")
                    Thread.Sleep(1500)
                    Return
                Catch
                End Try
            End If
        Next
    Catch ex As Exception
        Console.WriteLine($"ダイアログ処理エラー: {ex.Message}")
    End Try
End Sub
```

## トラブルシューティング

### 問題1: 要素が見つからない

**原因**:
- AutomationIdが間違っている
- ページ遷移が完了していない
- ウィンドウが画面外にある

**対策**:
1. `DumpAllControls()` で実際のコントロールを確認
2. `Thread.Sleep()` で待機時間を増やす
3. ウィンドウ位置を確認・移動

### 問題2: クリックが効かない

**原因**:
- ウィンドウが画面外にある
- コントロールが無効化されている
- フォーカスが当たっていない

**対策**:
```vb
' ウィンドウを画面内に移動
If currentLoc.X < 0 OrElse currentLoc.Y < 0 Then
    mainWindow.DisplayState = DisplayState.Maximized
    Thread.Sleep(500)
    mainWindow.DisplayState = DisplayState.Restored
    Thread.Sleep(500)
End If

' フォーカスを当ててからクリック
control.Focus()
Thread.Sleep(200)
control.Click()
```

### 問題3: 入力ができない

**対策**:
```vb
' 方法1: Textプロパティで直接設定
textBox.Text = "入力値"

' 方法2: SendKeysで入力
textBox.Focus()
WinForms.SendKeys.SendWait("^a")  ' Ctrl+A（全選択）
WinForms.SendKeys.SendWait("入力値")
```

## ベストプラクティスまとめ

### ✅ やるべきこと

1. **ソースコードを活用**: Designer.vbからAutomationIdと座標を確認
2. **多層防御**: AutomationId → 座標範囲 → 全ダンプ の順で検索
3. **詳細なログ**: 各ステップで`Console.WriteLine()`を出力
4. **スクリーンショット**: 操作前後とエラー時に撮影
5. **適切な待機**: `Thread.Sleep()` でページ遷移やダイアログを待機
6. **try-catch**: エラー時のスクリーンショットとログ出力

### ❌ 避けるべきこと

1. **絶対座標への依存**: 画面解像度やDPI設定で変わる
2. **待機時間なし**: UIの描画が間に合わない
3. **エラーハンドリングなし**: 失敗時の原因特定が困難
4. **ログなし**: デバッグに時間がかかる

## 実行方法

```bash
# プロジェクトをビルド
dotnet build

# テスト実行
dotnet test --logger "console;verbosity=detailed"

# 特定のテストのみ実行
dotnet test --filter "FullyQualifiedName~YourTestClassName.YourTestMethod"
```

## 参考リソース

- [TestStack.White ドキュメント](https://github.com/TestStack/White)
- [UI Automation API](https://docs.microsoft.com/en-us/windows/win32/winauto/entry-uiauto-win32)
- Inspect.exe（Windows SDKツール）: UI要素のAutomationIdを調査

## サンプルプロジェクト

実装例: [`E2ETests/TextBoxControlTest.vb`](../E2ETests/TextBoxControlTest.vb)
