# SimpleTextTemplate

[![Build(.NET)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml/badge.svg)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/SimpleTextTemplate.Generator?color=0078d4&label=NuGet)](https://www.nuget.org/packages/SimpleTextTemplate.Generator/)
[![Azure Artifacts](https://feeds.dev.azure.com/finphie/7af9aa4d-c550-43af-87a5-01539b2d9934/_apis/public/Packaging/Feeds/DotNet/Packages/24cf531b-b173-4efd-a808-f68234d28e3d/Badge)](https://dev.azure.com/finphie/Main/_artifacts/feed/DotNet/NuGet/SimpleTextTemplate.Generator?preferRelease=true)

SimpleTextTemplateは、変数の埋め込みのみに対応したテキストテンプレートエンジンです。

> [!CAUTION]
> SimpleTextTemplate.Generatorを使用する場合、.NET SDK 8.0.300-preview.24203.14や9.0.100-preview.3.24204.13ではインターセプターの不具合により正常に動作しません。8.0.2xxをご使用ください。

## 説明

- 文字列をUTF-8バイト列として`IBufferWriter<byte>`に出力します。
- `{{ <変数>:<format>:<culture> }}`で変数を埋め込みます。（`format`と`culture`は省略可能）
- `{{`と`}}`内の先頭と末尾の空白（U+0020）は無視されます。
- `{{`と`}}`で囲まれた範囲以外の文字は、そのまま出力されます。

## インストール

### NuGet（正式リリース版）

```shell
dotnet add package SimpleTextTemplate.Generator
```

### Azure Artifacts（開発用ビルド）

```shell
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

using (var writer = TemplateWriter.Create(_bufferWriter))
{
    writer.Write("{{ DateTimeOffsetValue:o }}_{{ StringValue }}!", in context);
    writer.Write("_{{ ConstantString }}_{{ ConstantInt:N3:ja-JP }}_{{ IntValue }}", in context, CultureInfo.InvariantCulture);
}

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
    [InterceptsLocation]
    public static void Write0(this ref TemplateWriter<ArrayPoolBufferWriter<byte>> writer, string _, in SampleContext context, IFormatProvider? provider = null)
    {
        writer.WriteValue(Unsafe.AsRef(in context).@DateTimeOffsetValue, "o", CultureInfo.InvariantCulture);
        writer.WriteConstantLiteral("_"u8);
        writer.WriteString(Unsafe.AsRef(in context).@StringValue);
        writer.WriteConstantLiteral("!"u8);
    }

    [InterceptsLocation]
    public static void Write1(this ref TemplateWriter<ArrayPoolBufferWriter<byte>> writer, string _, in SampleContext context, IFormatProvider? provider = null)
    {
        writer.WriteConstantLiteral("_Hello_999.000_"u8);
        writer.WriteValue(Unsafe.AsRef(in context).@IntValue, default, CultureInfo.InvariantCulture);
    }
}
```

<details>
<summary>SimpleTextTemplate.Renderer（非推奨）</summary>

### SimpleTextTemplate.Renderer（非推奨）

[SimpleTextTemplate.Renderer](https://www.nuget.org/packages/SimpleTextTemplate.Renderer/)と[SimpleTextTemplate.Contexts](https://www.nuget.org/packages/SimpleTextTemplate.Contexts/)への参照が必要です。

```csharp
using System;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;
using SimpleTextTemplate.Contexts;
using Utf8Utility;

var symbols = new Utf8ArrayDictionary<Utf8Array>();
symbols.TryAdd((Utf8Array)"Identifier"u8.ToArray(), "Hello, World!"u8.ToArray());

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
var source = "{{ Identifier }}"u8.ToArray();
var template = Template.Parse(source);
template.Render(bufferWriter, Context.Create(symbols));

// Hello, World!
Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
```

</details>

## ベンチマーク

| Method                      | Mean        | Error      | Ratio  | Gen0   | Gen1   | Allocated |
|---------------------------- |------------:|-----------:|-------:|-------:|-------:|----------:|
| SimpleTextTemplate          |    38.51 ns |   0.149 ns |   1.95 | 0.0067 |      - |      56 B |
| SimpleTextTemplate_SG       |    18.83 ns |   0.425 ns |   1.00 | 0.0067 |      - |      56 B |
| Scriban                     | 8,532.18 ns | 128.391 ns | 434.20 | 3.6621 | 0.3357 |   30778 B |
| ScribanLiquid               | 6,945.50 ns |  34.946 ns | 352.62 | 3.9673 | 0.3891 |   33194 B |
| (Utf8.TryWrite)             |    22.42 ns |   0.470 ns |   1.18 | 0.0067 |      - |      56 B |
| (InterpolatedStringHandler) |    40.12 ns |   0.299 ns |   2.04 | 0.0105 |      - |      88 B |
| (Regex.Replace)             |   130.70 ns |   0.513 ns |   6.65 | 0.0105 |      - |      88 B |
| (string.Format)             |    52.18 ns |   1.083 ns |   2.74 | 0.0105 |      - |      88 B |
| (CompositeFormat)           |    37.80 ns |   0.696 ns |   1.94 | 0.0105 |      - |      88 B |

> [!Note]
> UTF-8またはUTF-16で出力  
> ()で囲まれているメソッドは正確には処理が異なるため、参考情報

[ベンチマークプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Benchmarks)

## サポートフレームワーク

.NET 8

## 作者

finphie

## ライセンス

MIT

## クレジット

このプロジェクトでは、次のライブラリ等を使用しています。

### ライブラリ

- [CommunityToolkit.Diagnostics](https://github.com/CommunityToolkit/dotnet)
- [CommunityToolkit.HighPerformance](https://github.com/CommunityToolkit/dotnet)
- [Microsoft.CodeAnalysis.CSharp](https://github.com/dotnet/roslyn)
- [Utf8Utility](https://github.com/finphie/Utf8Utility)

### テスト

- [FluentAssertions](https://github.com/fluentassertions/fluentassertions)
- [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [NuGet.Frameworks](https://github.com/NuGet/NuGet.Client)
- [xunit](https://github.com/xunit/xunit)
- [xunit.runner.visualstudio](https://github.com/xunit/visualstudio.xunit)

### アナライザー

- [DocumentationAnalyzers](https://github.com/DotNetAnalyzers/DocumentationAnalyzers)
- [IDisposableAnalyzers](https://github.com/DotNetAnalyzers/IDisposableAnalyzers)
- [Microsoft.CodeAnalysis.Analyzers](https://github.com/dotnet/roslyn-analyzers)
- [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers)
- [Microsoft.VisualStudio.Threading.Analyzers](https://github.com/Microsoft/vs-threading)
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

- [Microsoft.SourceLink.GitHub](https://github.com/dotnet/sourcelink)
- [PolySharp](https://github.com/Sergio0694/PolySharp)
