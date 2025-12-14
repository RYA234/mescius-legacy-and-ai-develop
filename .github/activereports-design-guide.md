# ActiveReports 帳票設計書作成ガイド

このガイドは、ActiveReports.NET 18のレポートファイル（.rdlx）から設計書を作成する際の標準手順を定義します。

## 目的

- rdlxファイルから人間可読な設計書を自動生成
- AI処理可能な構造化ドキュメントの作成
- 設計書とレポートファイルの同期管理

## テンプレート

[`ActiveReport/Report/report-design-template.md`](./report-design-template.md) を使用します。

## 自動生成手順

### 1. AIへの指示例

```
「【帳票名】.rdlx から設計書を作成してください。
テンプレートは report-design-template.md を使用。」
```

### 2. AIの処理フロー

1. **rdlxファイル解析**
   - ファイル読み込み（大容量の場合は分割）
   - 主要要素の抽出（TextBox, Table, Image, Line）
   - データフィールド（Fields!）の特定

2. **テンプレート記入**
   - 概要セクション: 帳票ID、名称、用紙設定
   - コントロール配置表: 位置、サイズ、スタイル
   - データ項目定義: フィールド名、型、形式
   - 計算式定義: Sum, IIF, RowNumber等の式
   - カラーパレット: 使用色の一覧化

3. **Mermaid図生成**
   - レイアウト構成図
   - データフロー図
4. **ファイル保存**
   - `【帳票名】_design.md` として保存

## 設計書の構成

各設計書には以下のセクションが含まれます：

### 必須セクション
- **概要**: 帳票ID、名称、フォーマット、用紙設定
- **レイアウト構成図**: Mermaid図による構造可視化
- **コントロール配置表**: 各要素の詳細仕様
- **データ項目定義**: フィールド一覧と型情報
- **計算式定義**: 式と適用範囲
- **カラーパレット**: 使用色とその用途
- **フォント定義**: フォント使用ルール

### オプションセクション
- **ワイヤーフレーム**: Mermaid/Excalidrawモックアップ
- **データフロー図**: データの流れの可視化
- **バージョン履歴**: 変更履歴の記録

## rdlx解析のポイント

### 主要タグ
```xml
<dd:Name>                    # 帳票名
<DataSetName>                # データソース
<Top>, <Left>                # 位置
<Width>, <Height>            # サイズ
<Value>                      # 値/式
<Style>                      # スタイル定義
  <BackgroundColor>
  <Color>
  <FontFamily>, <FontSize>
  <TextAlign>, <VerticalAlign>
  <BorderColor>, <BorderStyle>
```

### データフィールド
```
パターン: Fields!【FieldName】.Value
例: =Fields!CustomerName.Value+" 御中"
```

### 計算式
```
Sum(): 集計
IIF(): 条件分岐
RowNumber(): 行番号
```

## 注意事項

### 大容量ファイル対応
rdlxファイルが大きい場合は、Read ツールの `offset` と `limit` パラメータで分割読み込み。

### 単位の扱い
- cm（センチメートル）
- in（インチ）
- pt（ポイント）

混在している場合があるため、統一して記載。

### AI可読性
- 構造化データは**表形式**を使用
- 図は**Mermaid**で記述（PNG/SVG不可）
- 式は**バッククォート**でコード化


### インラインコメントでの活用

```vb
' TODO: この帳票の設計書を ActiveReport/Report/OrderForm_design.md に作成
```

