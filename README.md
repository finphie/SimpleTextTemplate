# SimpleTextTemplate

[![Build(.NET)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml/badge.svg)](https://github.com/finphie/SimpleTextTemplate/actions/workflows/build-dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/SimpleTextTemplate?color=0078d4&label=NuGet)](https://www.nuget.org/packages/SimpleTextTemplate/)
[![SimpleTextTemplate package in DotNet feed in Azure Artifacts](https://feeds.dev.azure.com/finphie/7af9aa4d-c550-43af-87a5-01539b2d9934/_apis/public/Packaging/Feeds/18cbb017-6f1d-41eb-b9a5-a6dbf411e3f7/Packages/07a7dc27-e20d-42fd-b8a6-5a219205bf87/Badge)](https://dev.azure.com/finphie/Main/_packaging?_a=package&feed=18cbb017-6f1d-41eb-b9a5-a6dbf411e3f7&package=07a7dc27-e20d-42fd-b8a6-5a219205bf87&preferRelease=true)

シンプルなテキストテンプレートエンジンです。

## 説明

SimpleTextTemplateは、文字列の置換のみに対応したテキストテンプレートエンジンです。

## インストール

### NuGet（正式リリース版）

```console
dotnet add package SimpleTextTemplate
```

### Azure Artifacts（開発用ビルド）

```console
dotnet add package SimpleTextTemplate -s https://pkgs.dev.azure.com/finphie/Main/_packaging/DotNet/nuget/v3/index.json
```

## 使い方

### コアライブラリ

```csharp
using System;
using System.Text;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate;
using SimpleTextTemplate.Contexts;
using Utf8Utility;

var symbols = new Utf8ArrayDictionary<Utf8Array>();
symbols.TryAdd((Utf8Array)"Identifier", (Utf8Array)"Hello, World!");

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
var source = Encoding.UTF8.GetBytes("<html><body>{{ Identifier }}</body></html>");
var template = Template.Parse(source);
template.RenderTo(bufferWriter, Context.Create(symbols));

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
```

### ソースジェネレーター

[サンプルプロジェクト](https://github.com/finphie/SimpleTextTemplate/tree/main/Source/SimpleTextTemplate.Sample)を参照してください。

## サポートフレームワーク

- .NET 6
- .NET Standard 2.0（ソースジェネレーターのみ）

## 作者

finphie

## ライセンス

MIT

## クレジット

このプロジェクトでは、次のライブラリ等を使用しています。

### ライブラリ

- [CommunityToolkit.HighPerformance](https://github.com/CommunityToolkit/dotnet)
- [Utf8Utility](https://github.com/finphie/Utf8Utility)

### テスト

- [FluentAssertions](https://fluentassertions.com/)
- [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest/)
- [xunit](https://github.com/xunit/xunit)

### アナライザー

- Microsoft.CodeAnalysis.NetAnalyzers (SDK組み込み)
- [Microsoft.VisualStudio.Threading.Analyzers](https://github.com/Microsoft/vs-threading)
- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

### その他

- [Microsoft.SourceLink.GitHub](https://github.com/dotnet/sourcelink)
