# Invoice_bluegray.rdlx - コントロールリスト

**レポート名**: Invoice_bluegray.rdlx  
**生成日時**: 2025年12月14日  
**データセット**: DataSet1

---

## コントロール一覧

| No | コントロール名 | タイプ | Top | Left | Width | Height | データバインディング/値 | スタイル概要 |
|----|---------------|--------|-----|------|-------|--------|------------------------|------------|
| 1 | txtTitle | Textbox | 0.383667cm | 0cm | 6.299212in | 2.3495cm | 請求書 | 背景色:#697683, 文字色:#efedea, フォント:IPAゴシック 36pt, 中央揃え |
| 2 | txtCustomerName | Textbox | 3.939666cm | 0cm | 6.7381cm | 0.75cm | =Fields!CustomerName.Value+" 御中" | 文字色:#505050, フォント:IPAゴシック 14pt, 下線:#697683 |
| 3 | txtGreeting | Textbox | 5.0615cm | 0cm | 6.938cm | 1.4393cm | 毎度ご利用ありがとうございます。下記の通りご請求申し上げます。 | 文字色:#505050, フォント:IPAゴシック 12pt |
| 4 | txtAmountLabel | Textbox | 6.776cm | 0cm | 3.0221cm | 0.9525cm | ご請求金額 | 文字色:#505050, フォント:IPAゴシック 16pt, 下揃え |
| 5 | txtTotalAmount | Textbox | 6.776cm | 3.586574cm | 3.1515cm | 0.9525cm | =Sum(Fields!UnitPrice.Value*Fields!Number.Value) | 文字色:#505050, フォント:IPAゴシック 14pt, 通貨形式(ja-JP), 下揃え |
| 6 | lineSeparator | Line | 7.725833cm | 0cm | 6.815667cm | 0cm | - | 線色:#697683 |
| 7 | txtCompanyName | Textbox | 3.939666cm | 7.566388cm | 5.668cm | 0.9525cm | サンプルデータ株式会社 | 文字色:#505050, フォント:IPAゴシック 14pt |
| 8 | txtCompanyAddress | Textbox | 4.892166cm | 7.566388cm | 5.2681cm | 2.0814cm | 〒980-0021<br>宮城県<br>仙台市青葉区中央9－99－9<br>仙台駅前ビル 15F | 文字色:#505050, フォント:IPAゴシック 12pt |
| 9 | txtCompanyContact | Textbox | 6.973566cm | 7.566388cm | 5.2681cm | 1.0113cm | TEL:022-999-1234<br>FAX:022-999-9876 | 文字色:#505050, フォント:IPAゴシック 12pt |
| 10 | imgCompanyLogo | Image | 3.939666cm | 13.22917cm | 2.6247cm | 2.5612cm | image (埋め込み画像) | Sizing:Fit |
| 11 | tblInvoiceDetails | Table | 8.448167cm | 0cm | 15.90117cm | 1.862667cm | DataSet1 | フォント:IPAゴシック, 線色:#697683 |
| 12 | txtBillNo | Textbox | 2.899833cm | 10.45633cm | 5.4398cm | 0.75cm | ="請求書番号：" + First(Fields!BillNo.Value, "DataSet1") | 文字色:#505050, フォント:IPAゴシック 10pt, 右揃え |
| 13 | txtBankInfo | Textbox | 21.844cm | 0cm | 15.8962cm | 2.7305cm | お振込先：<br>グレープ銀行 仙台支店 普通口座 1234567 サンプルデータ（カ | 文字色:#505050, フォント:IPAゴシック 10pt, 枠線:#505050 |

---

## tblInvoiceDetails (明細テーブル) の構造

### ヘッダー行

| セル名 | 値 | スタイル |
|--------|-----|---------|
| txtHeaderSlipNo | 伝票№ | 背景色:#697683, 文字色:#efedea, 中央揃え, 枠線:#697683 |
| txtHeaderDate | 日付 | 背景色:#697683, 文字色:#efedea, 中央揃え, 枠線:#697683 |
| txtHeaderProducts | 商品名 | 背景色:#697683, 文字色:#efedea, 中央揃え, 枠線:#697683 |
| txtHeaderUnitPrice | 単価 | 背景色:#697683, 文字色:#efedea, 中央揃え, 枠線:#697683 |
| txtHeaderNumber | 数量 | 背景色:#697683, 文字色:#efedea, 中央揃え, 枠線:#697683 |
| txtHeaderPrice | 金額 | 背景色:#697683, 文字色:#efedea, 中央揃え, 枠線:#697683 |

### 詳細行 (データ行)

| セル名 | データフィールド | 値/式 | スタイル |
|--------|----------------|-------|---------|
| txtDetailSlipNo | SlipNo | =Fields!SlipNo.Value | 背景色:=IIF((RowNumber() Mod 2), "Transparent","#E6E2DD"), 文字色:#505050, 枠線:#697683 |
| txtDetailDate | Date | =Fields!Date.Value | 背景色:=IIF((RowNumber() Mod 2), "Transparent","#E6E2DD"), 文字色:#505050, 日付形式(ja-JP), 枠線:#697683 |
| txtDetailProducts | Products | =Fields!Products.Value | 背景色:=IIF((RowNumber() Mod 2), "Transparent","#E6E2DD"), 文字色:#505050, 枠線:#697683 |
| txtDetailUnitPrice | UnitPrice | =Fields!UnitPrice.Value | 背景色:=IIF((RowNumber() Mod 2), "Transparent","#E6E2DD"), 文字色:#505050, 通貨形式c2(ja-JP), 右揃え, 枠線:#697683 |
| txtDetailNumber | Number | =Fields!Number.Value | 背景色:=IIF((RowNumber() Mod 2), "Transparent","#E6E2DD"), 文字色:#505050, 日付形式d(ja-JP), 右揃え, 枠線:#697683 |
| txtDetailPrice | Price | =Fields!Price.Value | 背景色:=IIF((RowNumber() Mod 2), "Transparent","#E6E2DD"), 文字色:#505050, 通貨形式c(ja-JP), 右揃え, 枠線:#697683 |

### tblInvoiceDetails カラム幅

| カラム | 幅 |
|--------|-----|
| 1 (伝票№) | 1.8cm |
| 2 (日付) | 2.2cm |
| 3 (商品名) | 5.499cm |
| 4 (単価) | 2cm |
| 5 (数量) | 2cm |
| 6 (金額) | 2.402167cm |

---

## 主要な計算式

| 計算式 | 使用箇所 | 説明 |
|--------|---------|------|
| =Fields!CustomerName.Value+" 御中" | txtCustomerName | 顧客名に「御中」を付加 |
| =Sum(Fields!UnitPrice.Value*Fields!Number.Value) | txtTotalAmount | 請求金額合計 (単価×数量の合計) |
| ="請求書番号：" + First(Fields!BillNo.Value, "DataSet1") | txtBillNo | 請求書番号の表示 |
| =IIF((RowNumber() Mod 2), "Transparent","#E6E2DD") | tblInvoiceDetails詳細行 | 交互行の背景色 (ストライプ) |

---

## カラーパレット

| 色コード | 用途 |
|---------|------|
| #697683 | ヘッダー背景色, 線色, 枠線色 |
| #efedea | ヘッダー文字色 |
| #505050 | 本文文字色, 枠線色 |
| #E6E2DD | テーブル交互行背景色 |

---

## データフィールド一覧

| フィールド名 | 型 | 説明 |
|------------|-----|------|
| ID | 数値 | レコードID |
| BillNo | 文字列 | 請求書番号 |
| SlipNo | 文字列 | 伝票番号 |
| CustomerID | 数値 | 顧客ID |
| CustomerName | 文字列 | 顧客名 |
| Products | 文字列 | 商品名 |
| Number | 数値 | 数量 |
| UnitPrice | 数値 | 単価 |
| Date | 日付 | 日付 |
| Price | 計算フィールド | 金額 (=UnitPrice*Number) |

---

## 埋め込み画像

| 名前 | MIMEタイプ | 説明 |
|------|-----------|------|
| image | image/png | ロゴ画像 (imgCompanyLogoで使用) |

---

## レポート設定

- **用紙サイズ**: 21cm × 29.7cm (A4縦)
- **余白**: 上下左右 2.5cm
- **方向**: Portrait (縦)

---

## 備考

- テーブルは固定高 (FixedHeight: 12.5095cm) で、空白行を自動的にページに埋めて表示 (RepeatBlankRows: FillPage)
- 孤立ヘッダー防止が有効 (PreventOrphanedHeader: true)
- データソースは JSON形式のサンプルデータを使用
