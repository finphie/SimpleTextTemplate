using System;

namespace SimpleTextTemplate
{
    /// <summary>
    /// HTML解析中に発生したエラーを表します。
    /// </summary>
    public sealed class HtmlParserException : Exception
    {
        /// <summary>
        /// <see cref="HtmlParserException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HtmlParserException()
        {
        }

        /// <summary>
        /// <see cref="HtmlParserException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="error">エラーを説明するメッセージ</param>
        /// <param name="position">エラーが発生した位置</param>
        public HtmlParserException(ParserError error, int position)
            : base($"Error: {error} at position: {position}")
        {
            Error = error;
            Position = position;
        }

        /// <summary>
        /// <see cref="HtmlParserException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public HtmlParserException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// <see cref="HtmlParserException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">内部例外</param>
        public HtmlParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// エラーのタイプを表します。
        /// </summary>
        public enum ParserError
        {
            /// <summary>
            /// 無効なオブジェクトフォーマット
            /// </summary>
            InvalidObjectFormat,

            /// <summary>
            /// '{{'から始まらない場合
            /// </summary>
            ExpectedStartObject,

            /// <summary>
            /// '}}'で終わらない場合
            /// </summary>
            ExpectedEndObject
        }

        /// <summary>
        /// 解析エラーを取得します。
        /// </summary>
        /// <value>
        /// 解析エラー
        /// </value>
        public ParserError Error { get; }

        /// <summary>
        /// HTML文字列のバイト位置を取得します。
        /// </summary>
        /// <value>
        /// バイト位置
        /// </value>
        public int Position { get; }
    }
}