# SimpleTextTemplate

[![Build(.NET)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml/badge.svg)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/SimpleTextTemplate?color=0078d4&label=NuGet)](https://www.nuget.org/packages/SimpleTextTemplate/)
[![Azure Artifacts](https://feeds.dev.azure.com/finphie/7af9aa4d-c550-43af-87a5-01539b2d9934/_apis/public/Packaging/Feeds/18cbb017-6f1d-41eb-b9a5-a6dbf411e3f7/Packages/07a7dc27-e20d-42fd-b8a6-5a219205bf87/Badge)](https://dev.azure.com/finphie/Main/_packaging?_a=package&feed=18cbb017-6f1d-41eb-b9a5-a6dbf411e3f7&package=07a7dc27-e20d-42fd-b8a6-5a219205bf87&preferRelease=true)

シンプルなテキストテンプレートエンジンです。

## 説明

SimpleTextTemplateは、識別子の置換のみに対応したテキストテンプレートエンジンです。

## インストール

ライブラリ名|説明
-|-
[SimpleTextTemplate](https://www.nuget.org/packages/SimpleTextTemplate/)|テンプレートの解析及びレンダリングを行います。
[SimpleTextTemplate.Abstractions](https://www.nuget.org/packages/SimpleTextTemplate/)|SimpleTextTemplateの抽象化です。
[SimpleTextTemplate.Contexts](https://www.nuget.org/packages/SimpleTextTemplate.Contexts/)|テンプレートのレンダリングで使用するコンテキストの作成を行います。
[SimpleTextTemplate.Generator](https://www.nuget.org/packages/SimpleTextTemplate.Generator/)|コンパイル時にテンプレートの解析を行うソースジェネレーターです。

### NuGet（正式リリース版）

```shell
dotnet add package SimpleTextTemplate
dotnet add package SimpleTextTemplate.Contexts
```

```shell
dotnet add package SimpleTextTemplate.Generator
```

### Azure Artifacts（開発用ビルド）

```shell
dotnet add package SimpleTextTemplate -s https://pkgs.dev.azure.com/finphie/Main/_packaging/DotNet/nuget/v3/index.json
dotnet add package SimpleTextTemplate.Contexts -s https://pkgs.dev.azure.com/finphie/Main/_packaging/DotNet/nuget/v3/index.json
```

```shell
dotnet add package SimpleTextTemplate.Generator -s https://pkgs.dev.azure.com/finphie/Main/_packaging/DotNet/nuget/v3/index.json
```

## 使い方

次の例では、外部のライブラリである[CommunityToolkit.HighPerformance](https://www.nuget.org/packages/CommunityToolkit.HighPerformance/)を参照しています。

### SimpleTextTemplate

```csharp
using System;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;
using SimpleTextTemplate.Contexts;
using Utf8Utility;

var symbols = new Utf8ArrayDictionary<Utf8Array>();
symbols.TryAdd((Utf8Array)"Identifier"u8.ToArray(), (Utf8Array)"Hello, World!"u8.ToArray());

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
var source = "{{ Identifier }}"u8.ToArray();
var template = Template.Parse(source);
template.Render(bufferWriter, Context.Create(symbols));

// Hello, World!
Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
```

### SimpleTextTemplate.Generator

```csharp
using System;
using System.Buffers;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

var context = new SampleContext("Hello, World!"u8.ToArray());

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
ZTemplate.Render(bufferWriter, context);

// Hello, World!
Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

readonly record struct SampleContext(byte[] Identifier);

static partial class ZTemplate
{
    // TemplateAttributeでは、テンプレート文字列を指定してください。
    [Template("{{ Identifier }}")]  
    public static partial void Render(IBufferWriter<byte> bufferWriter, SampleContext context);

    // TemplateFileAttributeでは、指定されたファイルを解析します。
    // 相対パスを指定した場合、プロジェクトファイルが存在するディレクトリが基準となります。
    // 基準ディレクトリを変更する場合は、プロジェクトファイルで次のように設定します。
    // <PropertyGroup>
    //     <SimpleTextTemplatePath><!-- 任意のパス --></SimpleTextTemplatePath>
    // </PropertyGroup>
    [TemplateFile("template.txt")]
    public static partial void Render2(IBufferWriter<byte> bufferWriter, SampleContext context);
}
```

[サンプルプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Sample)

## ベンチマーク

### レンダリング

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1526 (21H1/May2021Update)
AMD Ryzen 7 5800U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.200-preview.22055.15
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  
```

|                                Method |         Mean |     Error |    StdDev |  Ratio | RatioSD |  Gen 0 |  Gen 1 | Allocated |
|-------------------------------------- |-------------:|----------:|----------:|-------:|--------:|-------:|-------:|----------:|
|                    SimpleTextTemplate |     89.95 ns |  0.674 ns |  0.631 ns |   1.45 |    0.01 | 0.0105 |      - |      88 B |
|                SimpleTextTemplateUtf8 |     74.41 ns |  0.201 ns |  0.188 ns |   1.20 |    0.01 | 0.0095 |      - |      80 B |
|     SimpleTextTemplateSourceGenerator |     71.16 ns |  0.377 ns |  0.352 ns |   1.14 |    0.01 | 0.0105 |      - |      88 B |
| SimpleTextTemplateSourceGeneratorUtf8 |     62.16 ns |  0.359 ns |  0.336 ns |   1.00 |    0.00 | 0.0095 |      - |      80 B |
|                               Scriban | 10,026.03 ns | 19.977 ns | 18.686 ns | 161.31 |    0.87 | 3.6469 | 0.3204 |  30,542 B |
|                         ScribanLiquid |  8,569.52 ns | 35.784 ns | 31.721 ns | 137.92 |    0.78 | 3.9368 | 0.3662 |  32,952 B |
|                                 Regex |    107.15 ns |  0.510 ns |  0.477 ns |   1.72 |    0.01 | 0.0057 |      - |      48 B |

[ベンチマークプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Benchmarks)

## サポートフレームワーク

- .NET 8

## 作者

finphie

## ライセンス

MIT

## クレジット

このプロジェクトでは、次のライブラリ等を使用しています。

### ライブラリ

- [CommunityToolkit.HighPerformance](https://github.com/CommunityToolkit/dotnet)
- [Humanizer.Core](https://github.com/Humanizr/Humanizer)
- [Microsoft.CodeAnalysis.Analyzers](https://github.com/dotnet/roslyn-analyzers)
- [Microsoft.CodeAnalysis.CSharp.Workspaces](https://github.com/dotnet/roslyn)
- [Utf8Utility](https://github.com/finphie/Utf8Utility)

### テスト

- [FluentAssertions](https://github.com/fluentassertions/fluentassertions)
- [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [NuGet.Frameworks](https://github.com/NuGet/NuGet.Client)
- [xunit](https://github.com/xunit/xunit)
- [xunit.runner.visualstudio](https://github.com/xunit/visualstudio.xunit)

### アナライザー

- Microsoft.CodeAnalysis.NetAnalyzers (SDK組み込み)
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
