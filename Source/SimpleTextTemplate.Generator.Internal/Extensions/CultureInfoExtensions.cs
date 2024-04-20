using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static System.Globalization.CultureInfo;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// <see cref="CultureInfo"/>に関する拡張メソッド集です。
/// </summary>
static class CultureInfoExtensions
{
    /// <summary>
    /// 指定されたカルチャーが特別なカルチャーかどうかを判定します。
    /// </summary>
    /// <param name="culture">カルチャー</param>
    /// <param name="cultureName">完全修飾子のカルチャー名</param>
    /// <returns>
    /// <see cref="InvariantCulture"/>や<see cref="CurrentCulture"/>、<see cref="CurrentUICulture"/>の場合は<see langword="true"/>を返します。
    /// それ以外の場合は<see langword="false"/>を返します。
    /// </returns>
    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:アナライザーに対して禁止された API を使用しない", Justification = "カルチャーの特定に使用するだけなので問題ない。")]
    public static bool IsSpecialCulture(this CultureInfo culture, [NotNullWhen(true)] out string? cultureName)
    {
        if (culture == InvariantCulture)
        {
            cultureName = "global::System.Globalization.CultureInfo.InvariantCulture";
            return true;
        }

        if (culture == CurrentCulture)
        {
            cultureName = "global::System.Globalization.CultureInfo.CurrentCulture";
            return true;
        }

        if (culture == CurrentUICulture)
        {
            cultureName = "global::System.Globalization.CultureInfo.CurrentUICulture";
            return true;
        }

        cultureName = null;
        return false;
    }
}
