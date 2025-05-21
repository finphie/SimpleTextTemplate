using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static System.Globalization.CultureInfo;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// <see cref="IFormatProvider"/>に関する拡張メソッド集です。
/// </summary>
static class IFormatProviderExtensions
{
    /// <summary>
    /// カルチャー名を取得します。
    /// </summary>
    /// <param name="provider">カルチャー</param>
    /// <returns>
    /// <see cref="CultureInfo.Name"/>または<see cref="object.ToString"/>を返します。
    /// </returns>
    public static string GetName(this IFormatProvider provider)
        => provider is CultureInfo culture ? culture.Name : provider.ToString();

    /// <summary>
    /// カルチャー文字列を取得します。
    /// </summary>
    /// <param name="culture">カルチャー</param>
    /// <returns>
    /// <see cref="InvariantCulture"/>や<see cref="CurrentCulture"/>、<see cref="CurrentUICulture"/>の場合はそのオブジェクトの完全修飾子を返します。
    /// それ以外の場合は<see cref="CultureInfo.Name"/>または<see cref="object.ToString"/>を返します。
    /// </returns>
    /// <exception cref="InvalidOperationException">カルチャー名を取得できませんでした。</exception>
    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:アナライザーに対して禁止された API を使用しない", Justification = "カルチャーの特定に使用するだけなので問題ない。")]
    public static string ToFullString(this IFormatProvider culture)
    {
        if (culture == InvariantCulture)
        {
            return "global::System.Globalization.CultureInfo.InvariantCulture";
        }

        if (culture == CurrentCulture)
        {
            return "global::System.Globalization.CultureInfo.CurrentCulture";
        }

        if (culture == CurrentUICulture)
        {
            return "global::System.Globalization.CultureInfo.CurrentUICulture";
        }

        var name = culture.GetName().Replace("-", string.Empty);

        return string.IsNullOrEmpty(name)
            ? throw new InvalidOperationException("カルチャー名を取得できませんでした。")
            : name;
    }

    /// <summary>
    /// <see cref="IFormatProvider"/>を取得します。
    /// </summary>
    /// <param name="provider">カルチャー</param>
    /// <param name="isDefaultInvariantCulture"><paramref name="provider"/>がnullの場合に<see cref="InvariantCulture"/>を返すかどうか</param>
    /// <returns>
    /// <paramref name="provider"/>がnull以外の場合は<paramref name="provider"/>を返します。
    /// <paramref name="provider"/>がnullかつ<paramref name="isDefaultInvariantCulture"/>がtrueの場合は<see cref="InvariantCulture"/>を返します。
    /// </returns>
    public static IFormatProvider? GetFormatProvider(this IFormatProvider? provider, bool isDefaultInvariantCulture)
    {
        return provider is not null ? provider : GetDefaultFormatProvider(isDefaultInvariantCulture);

        static IFormatProvider? GetDefaultFormatProvider(bool isDefaultInvariantCulture)
            => isDefaultInvariantCulture ? InvariantCulture : null;
    }
}
