using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate.Abstractions
{
    /// <summary>
    /// コンテキスト
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// 指定された識別子に関連付けられているUTF-8文字列を取得します。
        /// </summary>
        /// <param name="key">識別子名</param>
        /// <param name="value">指定された識別子に関連付けられているUTF-8文字列</param>
        /// <returns>
        /// 指定された識別子に関連付けられた文字列を取得できた場合は<see langword="true"/>、
        /// それ以外の場合は<see langword="false"/>。
        /// </returns>
        bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value);
    }
}