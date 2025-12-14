# GitHub Copilot カスタム指示

## プロジェクト概要

本プロジェクトは、AIとレガシーライブラリを活用したレガシーコードのモダナイゼーションを検証するプロジェクトです。

## コーディング規約

### 言語とフレームワーク

- **レガシーコード**: VB.NET (Windows Forms)
- **モダナイゼーションターゲット**: C#
- **主要ライブラリ**:
  - MultiRow for Windows Forms 12.0J
  - Inputman for Windows Forms 12.0J
  - ActiveReports.NET 18

### コーディングスタイル

#### VB.NET
- インデントは4スペース
- 変数名はキャメルケース（例: `customerName`）
- クラス名・メソッド名はパスカルケース（例: `CustomerService`）
- コメントは日本語で記述
- レガシーコードのパターンを維持し、段階的な改善を目指す

#### C# (モダナイゼーション)
- インデントは4スペース
- 最新のC#機能を積極的に活用（nullable参照型、パターンマッチングなど）
- XMLドキュメントコメントを使用
- 非同期処理は`async`/`await`を使用

### 命名規則

- ファイル名: パスカルケース（例: `CustomerForm.vb`, `OrderService.cs`）
- フォルダ名: パスカルケース（例: `Services`, `Models`）
- テストファイル: `{対象クラス名}Tests.vb/cs`

## AI活用方針

### コード解析時
- レガシーコードの構造を理解
- 依存関係の洗い出し
- リファクタリング候補の提示

### リファクタリング時
- 段階的な変更を優先
- テストコードの自動生成
- レガシーライブラリによる検証の組み込み

### モダナイゼーション
- VB.NETからC#への変換
- Windows FormsからWPF/Blazorへの移行検討
- デザインパターンの適用

## レガシーライブラリ活用

- データ検証ロジックの実装
- 単体テストの自動生成
- リファクタリング前後の動作保証

## コミットメッセージ

- 日本語で記述
- プレフィックスを使用:
  - `feat:` 新機能
  - `fix:` バグ修正
  - `refactor:` リファクタリング
  - `docs:` ドキュメント更新
  - `test:` テスト追加・修正
  - `chore:` その他の変更

## ActiveReports 帳票設計

ActiveReports.NET 18のレポートファイル（.rdlx）から設計書を作成する際は、以下のガイドに従ってください：

📘 **[ActiveReports 帳票設計書作成ガイド](./activereports-design-guide.md)**

### クイックスタート

```
@workspace 【帳票名】.rdlx から設計書を作成
```

### 主要リソース
- **設計書サンプル**: [`ActiveReport/Report/design.md`](../ActiveReport/Report/design.md)
- **設計ガイド**: [`activereports-design-guide.md`](./activereports-design-guide.md)

### 設計書の特徴
- **人間可読性**: 表形式とMermaid図で構造化
- **AI処理可能**: テンプレートベースで自動生成対応
- **Git管理**: Markdown形式でバージョン管理可能
- **モックアップ**: Excalidraw形式も生成可能

## カスタムコマンド

### /getReportConrolList

ActiveReports の .rdlx ファイルからコントロールリストを抽出し、一覧表示するコマンドです。

#### 使用方法

```
/getReportConrolList <帳票ファイル名.rdlx>
```

#### 機能

1. **rdlxファイル解析**
   - 指定されたレポートファイルを読み込み
   - 主要コントロール要素を抽出（TextBox, Table, Image, Line, Shape等）
   - 各コントロールの属性情報を取得

2. **出力内容**
   - コントロール名（Name）
   - コントロールタイプ（Type）
   - 位置（Top, Left）
   - サイズ（Width, Height）
   - データバインディング（Value/式）
   - スタイル情報（色、フォント等）

3. **出力形式**
   - Markdown表形式で整形
   - rdlxファイルと同じディレクトリに `<ファイル名>_controllist.md` として自動保存
   - 例: `Invoice_bluegray.rdlx` → `Invoice_bluegray_controllist.md`

#### 実行例

```
/getReportConrolList ActiveReport/Report/Invoice_bluegray.rdlx
```

#### 関連ドキュメント

- [ActiveReports 帳票設計書作成ガイド](./activereports-design-guide.md)
- [レポート設計サンプル](../ActiveReport/Report/design.md)

#### 注意事項

- 大容量ファイルの場合は分割読み込みを実施
- データフィールド（`Fields!` パターン）は自動で識別
- 計算式（`Sum()`, `IIF()` 等）も抽出対象

## 注意事項

- 本プロジェクトは検証プロジェクトです
- 実際の業務コードではなく、想定的なレガシーシステムのシミュレーションです
- 試用版ライブラリを使用しているため、ライセンスには注意してください

## Claude AI 用カスタム指示

Claude Code や Claude API を使用する場合は、以下のカスタム指示ファイルを参照してください：

📘 **[.claude.md](../.claude.md)**

このファイルには以下が含まれます：
- プロジェクト概要と技術スタック
- コーディング規約（VB.NET / C#）
- ActiveReports帳票設計の詳細手順
- AI活用方針とリファクタリングガイドライン
- コミットメッセージ規約
- よくある指示例

GitHub CopilotとClaude AIで同じ開発規約を共有できます。
