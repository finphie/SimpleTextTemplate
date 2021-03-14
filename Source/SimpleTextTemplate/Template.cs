using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SimpleTextTemplate.Abstractions;
using SimpleTextTemplate.Extensions;
using SimpleTextTemplate.Helpers;
using static SimpleTextTemplate.TemplateException;

namespace SimpleTextTemplate
{
    /// <summary>
    /// テンプレートを解析・レンダリングする構造体です。
    /// </summary>
    public readonly struct Template
    {
        readonly byte[] _source;
        readonly List<(BlockType Type, TextRange Range)> _blocks;

        Template(byte[] source)
        {
            _source = source;
            _blocks = new(16);
        }

        /// <summary>
        /// ブロック単位のバッファを取得します。
        /// </summary>
        /// <value>
        /// ブロック単位のバッファ
        /// </value>
        public IReadOnlyList<(BlockType Type, TextRange Range)> Blocks => _blocks.AsReadOnly();

        /// <summary>
        /// テンプレート文字列を解析します。
        /// </summary>
        /// <param name="source">テンプレート文字列</param>
        /// <returns><see cref="Template"/>構造体のインスタンス</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Template Parse(byte[] source)
        {
            var template = new Template(source);
            template.ParseInternal();
            return template;
        }

        /// <summary>
        /// テンプレートをレンダリングして、<see cref="IBufferWriter{Byte}"/>に書き込みます。
        /// </summary>
        /// <param name="bufferWriter">ターゲットの<see cref="IBufferWriter{Byte}"/></param>
        /// <param name="context">コンテキスト</param>
        public void RenderTo(IBufferWriter<byte> bufferWriter, IContext context)
        {
            var source = _source.AsSpan();
            var blocks = _blocks.AsSpan();

            foreach (ref readonly var block in blocks)
            {
                var value = source.Slice(block.Range.Start, block.Range.Length);

                switch (block.Type)
                {
                    case BlockType.Raw:
                        bufferWriter.Write(value);
                        break;
                    case BlockType.Identifier:
                        // TODO: パフォーマンス
                        context.TryGetValue(Encoding.UTF8.GetString(value.ToArray()), out var x);
                        bufferWriter.Write(x);
                        break;
                    case BlockType.None:
                    default:
                        ThrowHelper.ThrowTemplateParserException(ParserError.InvalidFormat, block.Range.Start);
                        break;
                }
            }
        }

        void ParseInternal()
        {
            var reader = new TemplateReader(_source);

            while ((reader.Read(out var value) is var type) && type != BlockType.None)
            {
                _blocks.Add((type, value));
            }
        }
    }
}