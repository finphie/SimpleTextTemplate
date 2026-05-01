# SimpleTextTemplate

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

[サンプルプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Sample)

### SimpleTextTemplate.Generator

```csharp
using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using SimpleTextTemplate;

var bufferWriter = new ArrayBufferWriter<byte>();
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

#### 生成コード

```csharp
file static class Intercept
{
    [global::System.Runtime.CompilerServices.InterceptsLocation(1, "...")]
    public static void Render0(ref global::SimpleTextTemplate.TemplateWriter<global::System.Buffers.ArrayBufferWriter<byte>> writer, string text, in global::SampleContext context, global::System.IFormatProvider provider = null)
    {
        writer.WriteValue(global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DateTimeOffsetValue, "o", global::System.Globalization.CultureInfo.InvariantCulture);
        writer.Grow(2
            + global::System.Text.Encoding.UTF8.GetMaxByteCount(
                global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringValue.Length));
        writer.DangerousWriteConstantLiteral("_"u8);
        writer.DangerousWriteString(global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringValue);
        writer.DangerousWriteConstantLiteral("!"u8);
    }

    [global::System.Runtime.CompilerServices.InterceptsLocation(1, "...")]
    public static void Render1(ref global::SimpleTextTemplate.TemplateWriter<global::System.Buffers.ArrayBufferWriter<byte>> writer, string text, in global::SampleContext context, global::System.IFormatProvider provider = null)
    {
        writer.Grow(15);
        writer.DangerousWriteConstantLiteral("_Hello_999.000_"u8);
        writer.WriteValue(global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@IntValue, default, global::System.Globalization.CultureInfo.InvariantCulture);
    }
}
```

### SimpleTextTemplate

```csharp
using System;
using System.Buffers;
using System.Text;
using SimpleTextTemplate;

var symbols = Context.Create();
symbols.Add("Identifier"u8.ToArray(), "Hello, World!"u8.ToArray());

var bufferWriter = new ArrayBufferWriter<byte>();
var source = "{{ Identifier }}"u8.ToArray();
var template = Template.Parse(source);
template.Render(bufferWriter, symbols);

// Hello, World!
Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
```

## ベンチマーク

### 定数（string）

| Method                       | Mean          | Error       | Ratio    | Gen0   | Gen1   | Allocated |
|----------------------------- |--------------:|------------:|---------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator |      4.664 ns |   0.0067 ns |     1.00 |      - |      - |         - |
| SimpleTextTemplate           |    100.016 ns |   0.1215 ns |    21.45 |      - |      - |         - |
| Utf8.TryWrite                |     26.145 ns |   0.0387 ns |     5.61 |      - |      - |         - |
| InterpolatedStringHandler    |     17.810 ns |   0.3476 ns |     3.82 | 0.0043 |      - |      72 B |
| CompositeFormat              |     40.057 ns |   0.2720 ns |     8.59 | 0.0043 |      - |      72 B |
| Regex                        |    152.400 ns |   0.2889 ns |    32.68 | 0.0043 |      - |      72 B |
| Scriban                      | 10,953.132 ns | 157.4480 ns | 2,348.54 | 3.6621 | 0.3662 |   62090 B |
| Scriban_Liquid               |  9,733.122 ns |  90.6237 ns | 2,086.95 | 3.8452 | 0.3662 |   64374 B |

### 定数（int）

| Method                       | Mean          | Error       | Ratio    | Gen0   | Gen1   | Allocated |
|----------------------------- |--------------:|------------:|---------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator |      4.627 ns |   0.0193 ns |     1.00 |      - |      - |         - |
| SimpleTextTemplate           |     71.926 ns |   0.0699 ns |    15.54 |      - |      - |         - |
| Utf8.TryWrite                |     14.638 ns |   0.0376 ns |     3.16 |      - |      - |         - |
| InterpolatedStringHandler    |     34.755 ns |   0.1168 ns |     7.51 | 0.0043 |      - |      72 B |
| CompositeFormat              |     49.985 ns |   0.7447 ns |    10.80 | 0.0043 |      - |      72 B |
| Regex                        |    158.656 ns |   0.4039 ns |    34.29 | 0.0062 |      - |     104 B |
| Scriban                      | 11,049.313 ns | 131.1204 ns | 2,387.86 | 3.6926 | 0.3662 |   62247 B |
| Scriban_Liquid               | 10,085.002 ns | 102.6479 ns | 2,179.46 | 3.8452 | 0.3662 |   64534 B |

### string

| Method                       | Mean         | Error      | Ratio  | Gen0   | Gen1   | Allocated |
|----------------------------- |-------------:|-----------:|-------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator |     29.73 ns |   0.069 ns |   1.00 |      - |      - |         - |
| SimpleTextTemplate           |     88.72 ns |   0.461 ns |   2.98 |      - |      - |         - |
| Utf8.TryWrite                |     27.01 ns |   0.021 ns |   0.91 |      - |      - |         - |
| InterpolatedStringHandler    |     26.65 ns |   0.132 ns |   0.90 | 0.0043 |      - |      72 B |
| CompositeFormat              |     41.19 ns |   0.120 ns |   1.39 | 0.0043 |      - |      72 B |
| Regex                        |    154.58 ns |   0.560 ns |   5.20 | 0.0043 |      - |      72 B |
| Scriban                      | 10,953.11 ns | 176.259 ns | 368.47 | 3.6621 | 0.3662 |   62103 B |
| Scriban_Liquid               |  9,955.17 ns | 135.368 ns | 334.90 | 3.8452 | 0.3662 |   64374 B |

### int

| Method                       | Mean         | Error      | Ratio  | Gen0   | Gen1   | Allocated |
|----------------------------- |-------------:|-----------:|-------:|-------:|-------:|----------:|
| SimpleTextTemplate.Generator |     24.41 ns |   0.053 ns |   1.00 |      - |      - |         - |
| SimpleTextTemplate           |     76.66 ns |   0.090 ns |   3.14 |      - |      - |         - |
| Utf8.TryWrite                |     20.20 ns |   0.052 ns |   0.83 |      - |      - |         - |
| InterpolatedStringHandler    |     35.54 ns |   0.162 ns |   1.46 | 0.0043 |      - |      72 B |
| CompositeFormat              |     49.87 ns |   0.135 ns |   2.04 | 0.0043 |      - |      72 B |
| Regex                        |    148.38 ns |   1.536 ns |   6.08 | 0.0062 |      - |     104 B |
| Scriban                      | 11,223.34 ns | 111.485 ns | 459.73 | 3.6621 | 0.2441 |   62255 B |
| Scriban_Liquid               |  9,870.61 ns | 127.730 ns | 404.32 | 3.8452 | 0.3662 |   64534 B |

[ベンチマークプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Benchmarks)

## サポートフレームワーク

- .NET 11
- .NET 10

## 作者

finphie

## ライセンス

MIT

## クレジット

このプロジェクトでは、次のパッケージ等を使用しています。

### ライブラリ

- [Microsoft.CodeAnalysis.CSharp](https://github.com/dotnet/roslyn)
- [System.IO.Hashing](https://github.com/dotnet/dotnet)

### テスト

- [Microsoft.CodeAnalysis.CSharp.Workspaces](https://github.com/dotnet/roslyn)
- [TUnit](https://github.com/thomhurst/TUnit)

### アナライザー

- [DocumentationAnalyzers](https://github.com/DotNetAnalyzers/DocumentationAnalyzers)
- [IDisposableAnalyzers](https://github.com/DotNetAnalyzers/IDisposableAnalyzers)
- [Microsoft.CodeAnalysis.Analyzers](https://github.com/dotnet/roslyn)
- [Microsoft.VisualStudio.Threading.Analyzers](https://github.com/Microsoft/vs-threading)
- [Roslynator.Analyzers](https://github.com/dotnet/roslynator)
- [Roslynator.Formatting.Analyzers](https://github.com/dotnet/roslynator)
- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

### ベンチマーク

- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)
- [Scriban](https://github.com/scriban/scriban)
