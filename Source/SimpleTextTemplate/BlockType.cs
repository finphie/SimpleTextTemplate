namespace SimpleTextTemplate
{
    /// <summary>
    /// ブロックのタイプを表します。
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// 空のブロック。
        /// </summary>
        None,

        /// <summary>
        /// HTMLブロック。
        /// </summary>
        Html,

        /// <summary>
        /// 変数ブロック。
        /// </summary>
        Object
    }
}