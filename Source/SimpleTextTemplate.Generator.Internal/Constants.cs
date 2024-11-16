namespace SimpleTextTemplate.Generator;

/// <summary>
/// 定数
/// </summary>
static class Constants
{
    /// <summary>
    /// Renderメソッドのパラメーター数
    /// </summary>
    public const int TemplateRenderParameterCount = 2;

    /// <summary>
    /// コンテキスト付きのRenderメソッドのパラメーター数
    /// </summary>
    public const int TemplateRenderParameterCountWithContext = 4;

    /// <summary>
    /// Renderメソッドのwriterのインデックス
    /// </summary>
    public const int TemplateRenderWriterIndex = 0;

    /// <summary>
    /// Renderメソッドのtextのインデックス
    /// </summary>
    public const int TemplateRenderTextIndex = 1;

    /// <summary>
    /// Renderメソッドのcontextのインデックス
    /// </summary>
    public const int TemplateRenderContextIndex = 2;

    /// <summary>
    /// RenderメソッドのIFormatProviderのインデックス
    /// </summary>
    public const int TemplateRenderIFormatProviderIndex = 3;
}
