using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// コンパイル結果を表すクラスです。
/// </summary>
/// <param name="Compilation">コンパイル情報</param>
/// <param name="Diagnostics">診断情報</param>
sealed record CompileResult(Compilation Compilation, IReadOnlyList<Diagnostic> Diagnostics);
