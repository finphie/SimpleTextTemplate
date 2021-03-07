using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate
{
    /// <summary>
    /// コンテキスト
    /// </summary>
    interface IContext
    {
        /// <summary>
        /// 指定された識別子にUTF-8文字列を関連付けます。
        /// </summary>
        /// <param name="key">識別子名</param>
        /// <param name="value">UTF-8文字列</param>
        void Add(string key, byte[] value);

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