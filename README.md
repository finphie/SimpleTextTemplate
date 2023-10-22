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
}
```

[サンプルプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Sample)

## ベンチマーク

### レンダリング

```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host] : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  No PGO : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  PGO    : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2

Runtime=.NET 8.0  

```

| Method                    | Job    | Mean        | Ratio  | Gen0   | Gen1   | Allocated | 
|-------------------------- |------- |------------:|-------:|-------:|-------:|----------:|-
| SimpleTextTemplate        | No PGO |    61.88 ns |   1.49 | 0.0067 |      - |      56 B | 
| SimpleTextTemplate_SG     | No PGO |    41.62 ns |   1.00 | 0.0067 |      - |      56 B | 
| Scriban                   | No PGO | 9,686.50 ns | 232.76 | 3.6774 | 0.3510 |   30778 B | 
| ScribanLiquid             | No PGO | 8,648.37 ns | 207.81 | 3.9673 | 0.3815 |   33194 B | 
| (Regex.Replace)           | No PGO |   150.77 ns |   3.62 | 0.0105 |      - |      88 B | 
| (string.Format)           | No PGO |    56.61 ns |   1.36 | 0.0105 |      - |      88 B | 
| (CompositeFormat)         | No PGO |    42.79 ns |   1.03 | 0.0105 |      - |      88 B | 
| (Utf8String.Format)       | No PGO |    63.07 ns |   1.52 | 0.0067 |      - |      56 B | 
| (Utf8String.CreateWriter) | No PGO |    41.90 ns |   1.01 | 0.0067 |      - |      56 B | 
|                           |        |             |        |        |        |           | 
| SimpleTextTemplate        | PGO    |    41.64 ns |   1.50 | 0.0067 |      - |      56 B | 
| SimpleTextTemplate_SG     | PGO    |    27.84 ns |   1.00 | 0.0067 |      - |      56 B | 
| Scriban                   | PGO    | 8,685.00 ns | 312.03 | 3.6621 | 0.3357 |   30778 B | 
| ScribanLiquid             | PGO    | 7,659.60 ns | 283.28 | 3.9673 | 0.3891 |   33194 B | 
| (Regex.Replace)           | PGO    |   133.82 ns |   4.80 | 0.0105 |      - |      88 B | 
| (string.Format)           | PGO    |    52.77 ns |   1.97 | 0.0105 |      - |      88 B | 
| (CompositeFormat)         | PGO    |    37.94 ns |   1.36 | 0.0105 |      - |      88 B | 
| (Utf8String.Format)       | PGO    |    53.61 ns |   1.92 | 0.0067 |      - |      56 B | 
| (Utf8String.CreateWriter) | PGO    |    37.31 ns |   1.34 | 0.0067 |      - |      56 B | 

> [!Note]
> UTF-8またはUTF-16で出力
> ()で囲まれているメソッドは正確には処理が異なるため、参考情報

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
- [Utf8StringInterpolation](https://github.com/Cysharp/Utf8StringInterpolation)

### その他

- [Microsoft.SourceLink.GitHub](https://github.com/dotnet/sourcelink)
