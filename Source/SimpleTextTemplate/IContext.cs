#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace SimpleTextTemplate
{
    /// <summary>
    /// コンテキスト
    /// </summary>
    interface IContext
    {
        void Add(string key, byte[] value);

        bool TryGetValue(
            string key,
#if NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
            out byte[]? value);
    }
}