# SimpleTextTemplate

[![Build(.NET)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml/badge.svg)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/SimpleTextTemplate.Generator?color=0078d4&label=NuGet)](https://www.nuget.org/packages/SimpleTextTemplate.Generator/)
[![Azure Artifacts](https://feeds.dev.azure.com/finphie/7af9aa4d-c550-43af-87a5-01539b2d9934/_apis/public/Packaging/Feeds/DotNet/Packages/24cf531b-b173-4efd-a808-f68234d28e3d/Badge)](https://dev.azure.com/finphie/Main/_artifacts/feed/DotNet/NuGet/SimpleTextTemplate.Generator?preferRelease=true)

SimpleTextTemplateは、変数の埋め込みのみに対応したテキストテンプレートエンジンです。

## 説明

- 文字列をUTF-8バイト列として`IBufferWriter<byte>`に出力します。
- `{{ <変数>:<format>:<culture> }}`で変数を埋め込みます。（`format`と`culture`は省略可能）
- `{{`と`}}`内の先頭と末尾の空白（U+0020）は無視されます。
- `{{`と`}}`で囲まれた範囲以外の文字は、そのまま出力されます。

> [!Important]
> 一部のカルチャーではOSによって定数展開が変わります。

## インストール

### NuGet（正式リリース版）

```bash
dotnet add package SimpleTextTemplate.Generator
```

### Azure Artifacts（開発用ビルド）

```bash
dotnet add package SimpleTextTemplate.Generator -s https://pkgs.dev.azure.com/finphie/Main/_packaging/DotNet/nuget/v3/index.json
```

## 使い方

次の例では、外部のライブラリである[CommunityToolkit.HighPerformance](https://www.nuget.org/packages/CommunityToolkit.HighPerformance/)を参照しています。

### SimpleTextTemplate.Generator（推奨）

```csharp
using System;
using System.Globalization;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
var context = new SampleContext("Hello, World", 1000, new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));

var writer = TemplateWriter.Create(bufferWriter);
TemplateRenderer.Render(ref writer, "{{ DateTimeOffsetValue:o }}_{{ StringValue }}!", in context);
TemplateRenderer.Render(ref writer, "_{{ ConstantString }}_{{ ConstantInt:N3:ja-JP }}_{{ IntValue }}", in context, CultureInfo.InvariantCulture);
writer.Flush();

// 2000-01-01T00:00:00.0000000+00:00_Hello, World!_Hello_999.000_1000
Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

readonly record struct SampleContext(
    string StringValue,
    int IntValue,
    DateTimeOffset DateTimeOffsetValue)
{
    public const string ConstantString = "Hello";
    public const int ConstantInt = 999;
}
```

[サンプルプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Sample)

#### 生成コード

```csharp
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

file static class Intercept
{
    [InterceptsLocation(1, "...")]
    public static void Write0(ref TemplateWriter<ArrayPoolBufferWriter<byte>> writer, string text, in SampleContext context, IFormatProvider? provider = null)
    {
        writer.WriteValue(Unsafe.AsRef(in context).@DateTimeOffsetValue, "o", CultureInfo.InvariantCulture);
        writer.WriteConstantLiteral("_"u8);
        writer.WriteString(Unsafe.AsRef(in context).@StringValue);
        writer.WriteConstantLiteral("!"u8);
    }

    [InterceptsLocation(1, "...")]
    public static void Write1(ref TemplateWriter<ArrayPoolBufferWriter<byte>> writer, string text, in SampleContext context, IFormatProvider? provider = null)
    {
        writer.WriteConstantLiteral("_Hello_999.000_"u8);
        writer.WriteValue(Unsafe.AsRef(in context).@IntValue, default, CultureInfo.InvariantCulture);
    }
}
```

<details>
<summary>SimpleTextTemplate（非推奨）</summary>

### SimpleTextTemplate.Renderer（非推奨）

[SimpleTextTemplate](https://www.nuget.org/packages/SimpleTextTemplate/)と[SimpleTextTemplate.Contexts](https://www.nuget.org/packages/SimpleTextTemplate.Contexts/)への参照が必要です。

```csharp
using System;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

var symbols = Context.Create();
symbols.Add("Identifier"u8.ToArray(), "Hello, World!"u8.ToArray());

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
var source = "{{ Identifier }}"u8.ToArray();
var template = Template.Parse(source);
template.Render(bufferWriter, symbols);

// Hello, World!
Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
```

</details>

## ベンチマーク

| Method                       | Categories      | Mean        | Error      | Ratio  | Gen0   | Gen1   | Allocated |
|----------------------------- |---------------- |------------:|-----------:|-------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator | Constant String |    13.92 ns |   0.155 ns |   1.00 | 0.0067 |      - |      56 B |
| (Utf8.TryWrite)              | Constant String |    21.84 ns |   0.172 ns |   1.57 | 0.0067 |      - |      56 B |
| (InterpolatedStringHandler)  | Constant String |    25.19 ns |   0.135 ns |   1.81 | 0.0105 |      - |      88 B |
| (CompositeFormat)            | Constant String |    29.75 ns |   0.380 ns |   2.14 | 0.0105 |      - |      88 B |
|                              |                 |              |           |        |        |        |           |
| SimpleTextTemplate.Generator | Constant Int    |    12.77 ns |   0.043 ns |   1.00 | 0.0048 |      - |      40 B |
| (Utf8.TryWrite)              | Constant Int    |    12.86 ns |   0.086 ns |   1.01 | 0.0048 |      - |      40 B |
| (InterpolatedStringHandler)  | Constant Int    |    27.47 ns |   0.302 ns |   2.15 | 0.0067 |      - |      56 B |
| (CompositeFormat)            | Constant Int    |    28.08 ns |   0.147 ns |   2.20 | 0.0067 |      - |      56 B |
|                              |                 |              |           |        |        |        |           |
| SimpleTextTemplate.Generator | String          |    29.41 ns |   0.144 ns |   1.00 | 0.0057 |      - |      48 B |
| SimpleTextTemplate           | String          |    59.91 ns |   0.247 ns |   2.04 | 0.0057 |      - |      48 B |
| Scriban                      | String          | 8,455.09 ns |  69.601 ns | 287.50 | 3.6621 | 0.3052 |   31003 B |
| Liquid                       | String          | 6,846.45 ns |  97.413 ns | 232.80 | 3.9902 | 0.4044 |   33418 B |
| (Utf8.TryWrite)              | String          |    21.79 ns |   0.184 ns |   0.74 | 0.0057 |      - |      48 B |
| (InterpolatedStringHandler)  | String          |    26.39 ns |   0.300 ns |   0.90 | 0.0086 |      - |      72 B |
| (CompositeFormat)            | String          |    26.91 ns |   0.230 ns |   0.91 | 0.0086 |      - |      72 B |
|                              |                 |              |           |        |        |        |           |
| SimpleTextTemplate.Generator | Int             |    20.93 ns |   0.142 ns |   1.00 | 0.0057 |      - |      48 B |
| SimpleTextTemplate           | Int             |    56.40 ns |   0.185 ns |   2.69 | 0.0057 |      - |      48 B |
| Scriban                      | Int             | 8,439.46 ns |  66.766 ns | 403.25 | 3.6621 | 0.3052 |   31027 B |
| Liquid                       | Int             | 6,724.20 ns |  77.275 ns | 321.29 | 3.9978 | 0.4425 |   33442 B |
| (Utf8.TryWrite)              | Int             |    15.45 ns |   0.094 ns |   0.74 | 0.0057 |      - |      48 B |
| (InterpolatedStringHandler)  | Int             |    28.04 ns |   0.181 ns |   1.34 | 0.0076 |      - |      64 B |
| (CompositeFormat)            | Int             |    32.21 ns |   0.330 ns |   1.54 | 0.0076 |      - |      64 B |

> [!Note]
> ()で囲まれているメソッドは正確には処理が異なるが、参考として記載。

[ベンチマークプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Benchmarks)

## サポートフレームワーク

.NET 9

## 作者

finphie

## ライセンス

MIT

## クレジット

このプロジェクトでは、次のライブラリ等を使用しています。

### ライブラリ

- [CommunityToolkit.HighPerformance](https://github.com/CommunityToolkit/dotnet)
- [Microsoft.CodeAnalysis.CSharp](https://github.com/dotnet/roslyn)

### テスト

- [Shouldly](https://github.com/shouldly/shouldly)
- [xunit.v3](https://github.com/xunit/xunit)

### アナライザー

- [DocumentationAnalyzers](https://github.com/DotNetAnalyzers/DocumentationAnalyzers)
- [IDisposableAnalyzers](https://github.com/DotNetAnalyzers/IDisposableAnalyzers)
- [Microsoft.CodeAnalysis.Analyzers](https://github.com/dotnet/roslyn-analyzers)
- [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers)
- [Microsoft.VisualStudio.Threading.Analyzers](https://github.com/Microsoft/vs-threading)
- [Roslynator.Analyzers](https://github.com/dotnet/roslynator)
- [Roslynator.Formatting.Analyzers](https://github.com/dotnet/roslynator)
- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

### ベンチマーク

- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)
- [CommandLineParser](https://github.com/commandlineparser/commandline)
- [Iced](https://github.com/icedland/iced)
- [Microsoft.CodeAnalysis.CSharp](https://github.com/dotnet/roslyn)
- [Microsoft.Diagnostics.NETCore.Client](https://github.com/dotnet/diagnostics)
- [Microsoft.Diagnostics.Runtime](https://github.com/Microsoft/clrmd)
- [Microsoft.Diagnostics.Tracing.TraceEvent](https://github.com/Microsoft/perfview)
- [Perfolizer](https://github.com/AndreyAkinshin/perfolizer)
- [Scriban](https://github.com/scriban/scriban)

### その他

- [PolySharp](https://github.com/Sergio0694/PolySharp)
