namespace SimpleTextTemplate.Sample;

/// <summary>
/// コンテキスト
/// </summary>
/// <param name="Identifier">識別子</param>
readonly record struct SampleContext(byte[] Identifier);
