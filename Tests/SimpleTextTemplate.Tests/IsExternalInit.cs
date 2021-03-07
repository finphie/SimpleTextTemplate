#if NET48
using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// .NET Standard 2.0ではIsExternalInitが定義されていないため、自前で定義する。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    static class IsExternalInit
    {
    }
}
#endif