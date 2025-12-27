Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TestStack.White
Imports TestStack.White.UIItems.Finders
Imports TestStack.White.UIItems.WindowItems
Imports TestStack.White.UIItems
Imports TestStack.White.UIItems.TreeItems
Imports System.IO
Imports System.Threading
Imports System.Drawing
Imports System.Drawing.Imaging

Namespace E2ETests
    <TestClass>
    Public Class Test1


        <TestMethod>
        Sub TestTextBoxInputWithFormatRestriction()
            ' InputManアプリケーションのパスを取得
            Dim solutionDir As String = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."))
            Dim appPath As String = Path.Combine(solutionDir, "InputMan", "bin", "x86", "Debug", "InputManWin12_Demo.exe")

            ' ファイルが存在することを確認
            Assert.IsTrue(File.Exists(appPath), $"アプリケーションが見つかりません: {appPath}")

            Dim app As Application = Nothing
            Try
                Console.WriteLine("========================================")
                Console.WriteLine("テスト開始: テキストコントロールへの入力テスト")
                Console.WriteLine("========================================")

                ' InputManアプリケーションを起動
                Console.WriteLine($"アプリケーションを起動: {appPath}")
                app = Application.Launch(appPath)
                Thread.Sleep(2000)

                ' メインウィンドウを取得
                Dim windows = app.GetWindows()
                Assert.IsTrue(windows.Count > 0, "ウィンドウが取得できませんでした")

                Dim mainWindow As Window = windows(0)
                Assert.IsNotNull(mainWindow, "メインウィンドウがnullです")
                Console.WriteLine($"メインウィンドウを取得: {mainWindow.Title}")

                ' TreeViewを取得
                Console.WriteLine("TreeViewを検索中...")
                Dim treeView As Tree = mainWindow.Get(Of Tree)(SearchCriteria.All)
                Assert.IsNotNull(treeView, "TreeViewが見つかりません")
                Console.WriteLine("TreeViewを取得しました")

                ' TreeViewの全ノードをログ出力
                Console.WriteLine("利用可能なTreeViewノード:")
                For Each node As TreeNode In treeView.Nodes
                    Console.WriteLine($"  - {node.Text}")
                Next

                ' 「書式による入力制限」ノードを検索して選択
                Console.WriteLine("「書式による入力制限」ノードを検索中...")
                Dim formatRestrictionNode As TreeNode = Nothing
                For Each node As TreeNode In treeView.Nodes
                    If node.Text.Contains("書式による入力制限") OrElse node.Text.Contains("Format") Then
                        formatRestrictionNode = node
                        Exit For
                    End If
                Next

                Assert.IsNotNull(formatRestrictionNode, "「書式による入力制限」ノードが見つかりません")
                Console.WriteLine($"「書式による入力制限」ノードを発見: {formatRestrictionNode.Text}")

                ' ノードを展開
                formatRestrictionNode.Expand()
                Thread.Sleep(500)
                Console.WriteLine("ノードを展開しました")

                ' 子ノードをログ出力
                Console.WriteLine("「書式による入力制限」の子ノード:")
                For Each childNode As TreeNode In formatRestrictionNode.Nodes
                    Console.WriteLine($"  - {childNode.Text}")
                Next

                ' 「テキストコントロール」ノードを検索して選択
                Console.WriteLine("「テキストコントロール」ノードを検索中...")
                Dim textControlNode As TreeNode = Nothing
                For Each childNode As TreeNode In formatRestrictionNode.Nodes
                    If childNode.Text.Contains("テキストコントロール") OrElse childNode.Text.Contains("Text") Then
                        textControlNode = childNode
                        Exit For
                    End If
                Next

                Assert.IsNotNull(textControlNode, "「テキストコントロール」ノードが見つかりません")
                Console.WriteLine($"「テキストコントロール」ノードを発見: {textControlNode.Text}")

                ' ノードを選択
                textControlNode.Select()
                Thread.Sleep(1000)
                Console.WriteLine("「テキストコントロール」ノードを選択しました")

                ' テキストボックス(AutomationId: gcTextBox1)を取得
                Console.WriteLine("テキストボックス(AutomationId: gcTextBox1)を検索中...")
                Dim textBox As TextBox = mainWindow.Get(Of TextBox)(SearchCriteria.ByAutomationId("gcTextBox1"))
                Assert.IsNotNull(textBox, "テキストボックス(gcTextBox1)が見つかりません")
                Console.WriteLine("テキストボックスを取得しました")

                ' テキストボックスに「2212」を入力
                Console.WriteLine("テキストボックスに「2212」を入力中...")
                textBox.Text = "2212"
                Thread.Sleep(500)
                Console.WriteLine($"入力完了: テキストボックスの値 = {textBox.Text}")

                ' スクリーンショットを撮影
                Dim screenshotDir As String = Path.Combine(solutionDir, "E2ETests", "Screenshots")
                If Not Directory.Exists(screenshotDir) Then
                    Directory.CreateDirectory(screenshotDir)
                    Console.WriteLine($"スクリーンショットディレクトリを作成: {screenshotDir}")
                End If

                Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
                Dim screenshotPath As String = Path.Combine(screenshotDir, $"TextBoxInput_{timestamp}.png")

                Console.WriteLine("スクリーンショットを撮影中...")
                Desktop.TakeScreenshot(screenshotPath, ImageFormat.Png)
                Console.WriteLine($"スクリーンショットを保存: {screenshotPath}")

                ' テスト成功をアサート
                Assert.AreEqual("2212", textBox.Text, "テキストボックスの値が期待値と異なります")
                Console.WriteLine("========================================")
                Console.WriteLine("テスト完了: すべての操作が正常に完了しました")
                Console.WriteLine("========================================")

            Finally
                ' アプリケーションを終了
                If app IsNot Nothing Then
                    Console.WriteLine("アプリケーションを終了します")
                    app.Close()
                    Thread.Sleep(1000)
                End If
            End Try
        End Sub
    End Class
End Namespace

