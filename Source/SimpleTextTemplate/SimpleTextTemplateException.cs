using System;

namespace SimpleTextTemplate
{
    /// <summary>
    /// テンプレート解析中に発生したエラーを表します。
    /// </summary>
    public sealed class SimpleTextTemplateException : Exception
    {
        /// <summary>
        /// <see cref="SimpleTextTemplateException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public SimpleTextTemplateException()
        {
        }

        /// <summary>
        /// <see cref="SimpleTextTemplateException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="error">エラーを説明するメッセージ</param>
        /// <param name="position">エラーが発生した位置</param>
        public SimpleTextTemplateException(ParserError error, int position)
            : base($"Error: {error} at position: {position}")
        {
            Error = error;
            Position = position;
        }

        /// <summary>
        /// <see cref="SimpleTextTemplateException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public SimpleTextTemplateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// <see cref="SimpleTextTemplateException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">内部例外</param>
        public SimpleTextTemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// エラーのタイプを表します。
        /// </summary>
        public enum ParserError
        {
            /// <summary>
            /// 無効な識別子フォーマット
            /// </summary>
            InvalidIdentifierFormat,

            /// <summary>
            /// '{{'から始まらない場合
            /// </summary>
            ExpectedStartToken,

            /// <summary>
            /// '}}'で終わらない場合
            /// </summary>
            ExpectedEndToken
        }

        /// <summary>
        /// 解析エラーを取得します。
        /// </summary>
        /// <value>
        /// 解析エラー
        /// </value>
        public ParserError Error { get; }

        /// <summary>
        /// テンプレート文字列のバイト位置を取得します。
        /// </summary>
        /// <value>
        /// バイト位置
        /// </value>
        public int Position { get; }
    }
}