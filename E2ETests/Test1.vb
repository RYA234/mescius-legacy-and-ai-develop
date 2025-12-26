Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports TestStack.White
Imports TestStack.White.UIItems.Finders
Imports TestStack.White.UIItems.WindowItems
Imports System.IO
Imports System.Threading

Namespace E2ETests
    <TestClass>
    Public Class Test1
        <TestMethod>
        Sub TestInputManApplicationLaunch()
            ' InputManアプリケーションのパスを取得
            ' BaseDirectoryはE2ETests\bin\Debug\net48なので、4階層上がソリューションルート
            Dim solutionDir As String = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."))
            Dim appPath As String = Path.Combine(solutionDir, "InputMan", "bin", "x86", "Debug", "InputManWin12_Demo.exe")

            ' ファイルが存在することを確認
            Assert.IsTrue(File.Exists(appPath), $"アプリケーションが見つかりません: {appPath}")

            Dim app As Application = Nothing
            Try
                ' InputManアプリケーションを起動
                app = Application.Launch(appPath)

                ' アプリケーションが起動するまで待機（最大10秒）
                Thread.Sleep(2000)

                ' メインウィンドウを取得
                Dim windows = app.GetWindows()
                Assert.IsTrue(windows.Count > 0, "ウィンドウが取得できませんでした")

                Dim mainWindow As Window = windows(0)
                Assert.IsNotNull(mainWindow, "メインウィンドウがnullです")
                Assert.IsFalse(String.IsNullOrEmpty(mainWindow.Title), "ウィンドウタイトルが空です")

                Console.WriteLine($"ウィンドウタイトル: {mainWindow.Title}")
                Console.WriteLine("InputManアプリケーションの起動に成功しました")

            Finally
                ' アプリケーションを終了
                If app IsNot Nothing Then
                    app.Close()
                    Thread.Sleep(1000)
                End If
            End Try
        End Sub
    End Class
End Namespace

