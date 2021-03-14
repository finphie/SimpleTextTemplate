using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleTextTemplate.Generator
{
    /// <summary>
    /// ソースジェネレーターで出力するソースを生成するクラスです。
    /// </summary>
    partial class CodeTemplate
    {
        readonly byte[] _source;

        /// <summary>
        /// <see cref="CodeTemplate"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public CodeTemplate(string filePath)
        {
            FilePath = filePath;
            _source = File.ReadAllBytes(FilePath);
        }

        /// <summary>
        /// ファイルパスを取得します。
        /// </summary>
        /// <value>
        /// ファイルパス
        /// </value>
        internal string FilePath { get; }

        /// <summary>
        /// ファイル名を取得します。
        /// </summary>
        /// <value>
        /// ファイル名
        /// </value>
        internal string FileName => Path.GetFileNameWithoutExtension(FilePath);

        IEnumerable<(BlockType Type, TextRange Range)> GetBlocks()
        {
            var source = Template.Parse(_source);
            return source.Blocks;
        }

        unsafe string ToIdentifierString(TextRange range)
        {
            var identifier = _source.AsSpan(range.Start, range.Length);

            fixed (byte* bytes = identifier)
            {
                return Encoding.UTF8.GetString(bytes, range.Length);
            }
        }

        string ToHexString(TextRange range)
            => string.Join(", ", _source.Skip(range.Start).Take(range.Length).Select(x => "0x" + x.ToString("x2", CultureInfo.InvariantCulture)));
    }
}