; Shipped analyzer releases <!-- markdownlint-disable -->

## Release 2.0.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
STT1000 | SimpleTextTemplate | Error | テンプレート文字列は定数にする必要があります。
STT1001 | SimpleTextTemplate | Error | テンプレート文字列が不正な形式です。
STT1002 | SimpleTextTemplate | Error | コンテキストに識別子が存在しません。
STT1003 | SimpleTextTemplate | Error | テンプレート解析時にエラーが発生しました。
STT1004 | SimpleTextTemplate | Error | IFormattableが実装されていない識別子に、formatまたはproviderが指定されています。
STT1005 | SimpleTextTemplate | Error | 列挙型識別子にproviderが指定されています。
STT1006 | SimpleTextTemplate | Error | IFormattable/ISpanFormattable/IUtf8Formattableのいずれかのインターフェイスを実装していない識別子に、formatまたはproviderが指定されています。
