namespace SimpleTextTemplate.Generator.Tests.Core;

/// <summary>
/// <see cref="IFormattable"/>を実装していないクラスです。
/// </summary>
/// <param name="Value">値</param>
public sealed record NonFormattable(string Value)
{
    /// <inheritdoc/>
    public override string ToString() => Value;
}
