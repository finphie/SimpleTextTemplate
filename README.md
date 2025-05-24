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
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

file static class Intercept
{
    [InterceptsLocation(1, "...")]
    public static void Render0(ref TemplateWriter<ArrayPoolBufferWriter<byte>> writer, string text, in SampleContext context, IFormatProvider? provider = null)
    {
        writer.WriteValue(Unsafe.AsRef(in context).@DateTimeOffsetValue, "o", CultureInfo.InvariantCulture);
        writer.Grow(2
            + Encoding.UTF8.GetMaxByteCount(
                Unsafe.AsRef(in context).@StringValue.Length));
        writer.DangerousWriteConstantLiteral("_"u8);
        writer.DangerousWriteString(Unsafe.AsRef(in context).@StringValue);
        writer.DangerousWriteConstantLiteral("!"u8);
    }

    [InterceptsLocation(1, "...")]
    public static void Render1(ref TemplateWriter<ArrayPoolBufferWriter<byte>> writer, string text, in SampleContext context, IFormatProvider? provider = null)
    {
        writer.Grow(15);
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

### 定数（string）

| Method                       | Mean     | Error    | Ratio | Gen0   | Allocated |
|----------------------------- |---------:|---------:|------:|-------:|----------:|
| SimpleTextTemplate.Generator | 13.53 ns | 0.110 ns |  1.00 | 0.0067 |      56 B |
| (Utf8.TryWrite)              | 25.12 ns | 0.164 ns |  1.86 | 0.0067 |      56 B |
| (InterpolatedStringHandler)  | 30.96 ns | 0.357 ns |  2.29 | 0.0105 |      88 B |
| (CompositeFormat)            | 29.98 ns | 0.468 ns |  2.22 | 0.0105 |      88 B |

### 定数（int）

| Method                       | Mean     | Error    | Ratio | Gen0   | Allocated |
|----------------------------- |---------:|---------:|------:|-------:|----------:|
| SimpleTextTemplate.Generator | 12.18 ns | 0.051 ns |  1.00 | 0.0048 |      40 B |
| (Utf8.TryWrite)              | 13.54 ns | 0.234 ns |  1.11 | 0.0048 |      40 B |
| (InterpolatedStringHandler)  | 32.67 ns | 0.487 ns |  2.68 | 0.0067 |      56 B |
| (CompositeFormat)            | 28.76 ns | 0.412 ns |  2.36 | 0.0067 |      56 B |

### string

| Method                       | Mean        | Error     | Ratio  | Gen0   | Gen1   | Allocated |
|----------------------------- |------------:|----------:|-------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator |    25.32 ns |  0.062 ns |   1.00 | 0.0057 |      - |      48 B |
| SimpleTextTemplate           |    61.39 ns |  0.530 ns |   2.42 | 0.0057 |      - |      48 B |
| Scriban                      | 8,339.53 ns | 46.900 ns | 329.31 | 3.7842 | 0.3662 |   32071 B |
| Scriban_Liquid               | 6,670.99 ns | 66.825 ns | 263.43 | 4.0131 | 0.3815 |   33602 B |
| (Utf8.TryWrite)              |    22.73 ns |  0.346 ns |   0.90 | 0.0057 |      - |      48 B |
| (InterpolatedStringHandler)  |    30.22 ns |  0.393 ns |   1.19 | 0.0086 |      - |      72 B |
| (CompositeFormat)            |    27.75 ns |  0.348 ns |   1.10 | 0.0086 |      - |      72 B |

### int

| Method                       | Mean        | Error     | Ratio  | Gen0   | Gen1   | Allocated |
|----------------------------- |------------:|----------:|-------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator |    19.00 ns |  0.067 ns |   1.00 | 0.0057 |      - |      48 B |
| SimpleTextTemplate           |    60.97 ns |  0.690 ns |   3.21 | 0.0057 |      - |      48 B |
| Scriban                      | 8,600.22 ns | 78.630 ns | 452.65 | 3.7842 | 0.3662 |   32095 B |
| Scriban_Liquid               | 6,794.65 ns | 98.124 ns | 357.62 | 4.0131 | 0.3967 |   33626 B |
| (Utf8.TryWrite)              |    17.98 ns |  0.313 ns |   0.95 | 0.0057 |      - |      48 B |
| (InterpolatedStringHandler)  |    34.09 ns |  0.439 ns |   1.79 | 0.0076 |      - |      64 B |
| (CompositeFormat)            |    29.13 ns |  0.352 ns |   1.53 | 0.0076 |      - |      64 B |

> [!Note]
> ()で囲まれているメソッドは参考として記載。

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

- [Microsoft.Testing.Extensions.CodeCoverage](https://github.com/microsoft/codecoverage)
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
