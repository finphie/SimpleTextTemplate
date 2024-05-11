using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Execute.Tests;

[SuppressMessage("Design", "CA1051:参照可能なインスタンス フィールドを宣言しません", Justification = "テストのため")]
[SuppressMessage("Performance", "CA1819:プロパティは配列を返すことはできません", Justification = "テストのため")]
[SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "テストのため")]
public record AccessibilityTestData
{
    internal static byte[] _internalStaticField = "InternalStaticField"u8.ToArray();
    internal byte[] _internalField = "InternalField"u8.ToArray();

    protected internal static readonly byte[] ProtectedInternalStaticField = "_ProtectedInternalStaticField"u8.ToArray();
    protected internal readonly byte[] ProtectedInternalField = "_ProtectedInternalField"u8.ToArray();

    internal static byte[] InternalStaticProperty => "_InternalStaticProperty"u8.ToArray();

    internal byte[] InternalProperty => "_InternalProperty"u8.ToArray();

    protected internal static byte[] ProtectedInternalStaticProperty => "_ProtectedInternalStaticProperty"u8.ToArray();

    protected internal byte[] ProtectedInternalProperty => "_ProtectedInternalProperty"u8.ToArray();
}
