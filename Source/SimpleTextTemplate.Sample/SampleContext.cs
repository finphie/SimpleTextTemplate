using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate.Sample;

/// <summary>
/// コンテキスト
/// </summary>
/// <param name="Identifier">識別子</param>
[SuppressMessage("Performance", "CA1819:プロパティは配列を返すことはできません", Justification = "配列である必要があるため。")]
readonly record struct SampleContext(byte[] Identifier);
