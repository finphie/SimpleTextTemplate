using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// <see cref="InterceptInfo"/>または<see cref="Diagnostic"/>のリストを表します。
/// </summary>
/// <param name="Info">インターセプターに必要な情報</param>
/// <param name="Diagnostics">診断情報</param>
sealed record InterceptInfoOrDiagnostic(InterceptInfo? Info, IReadOnlyList<Diagnostic> Diagnostics);
