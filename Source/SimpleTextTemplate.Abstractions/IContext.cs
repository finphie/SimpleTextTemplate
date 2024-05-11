using System.Diagnostics.CodeAnalysis;
using Utf8Utility;

namespace SimpleTextTemplate;

/// <summary>
/// コンテキスト
/// </summary>
public interface IContext
{
    /// <summary>
    /// 指定された識別子に関連付けられている値を取得します。
    /// </summary>
    /// <param name="key">識別子名</param>
    /// <param name="value">指定された識別子に関連付けられている値</param>
    /// <returns>
    /// 指定された識別子に関連付けられた値取得できた場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    bool TryGetValue(Utf8Array key, [MaybeNullWhen(false)] out object value);

    /// <summary>
    /// 指定された識別子に関連付けられている値を取得します。
    /// </summary>
    /// <param name="key">識別子名</param>
    /// <param name="value">指定された識別子に関連付けられている値</param>
    /// <returns>
    /// 指定された識別子に関連付けられた値を取得できた場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    bool TryGetValue(ReadOnlySpan<byte> key, [MaybeNullWhen(false)] out object value);
}
