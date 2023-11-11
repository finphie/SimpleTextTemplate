using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// <see cref="InterceptInfo"/>または<see cref="Microsoft.CodeAnalysis.Diagnostic"/>を表します。
/// </summary>
/// <param name="Info">インターセプターに必要な情報</param>
/// <param name="Diagnostic">診断情報</param>
sealed record InterceptInfoOrDiagnostic(InterceptInfo? Info = null, Diagnostic? Diagnostic = null);
