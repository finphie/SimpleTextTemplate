using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleTextTemplate
{
    /// <summary>
    /// テンプレートを解析するときに使用するキャッシュクラスです。
    /// </summary>
    sealed class TemplateCache
    {
        readonly byte[] _buffer;
        readonly List<ValueTuple<BlockType, Range>> _blocks;

        /// <summary>
        /// <see cref="TemplateCache"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="buffer">バッファ</param>
        public TemplateCache(byte[] buffer)
        {
            _buffer = buffer;
            _blocks = new(16);

            Parse();
        }

        /// <summary>
        /// バッファを取得します。
        /// </summary>
        /// <value>
        /// バッファ
        /// </value>
        public ReadOnlySpan<byte> Buffer => _buffer;

        /// <summary>
        /// ブロック単位のバッファを取得します。
        /// </summary>
        /// <value>
        /// ブロック単位のバッファ
        /// </value>
        internal ReadOnlySpan<(BlockType Type, Range Range)> Blocks => CollectionsMarshal.AsSpan(_blocks);

        /// <summary>
        /// 文字列コンテンツをレンダリングして、<see cref="IBufferWriter{Byte}"/>に書き込みます。
        /// </summary>
        /// <param name="bufferWriter">ターゲットの<see cref="IBufferWriter{Byte}"/></param>
        /// <param name="context">コンテキスト</param>
        public void RenderTo(IBufferWriter<byte> bufferWriter, IContext context)
        {
            var buffer = Buffer;

            foreach (ref readonly var block in Blocks)
            {
                var value = buffer[block.Range];

                switch (block.Type)
                {
                    case BlockType.Raw:
                        bufferWriter.Write(value);
                        break;
                    case BlockType.Identifier:
                        context.TryGetValue(Encoding.UTF8.GetString(value), out var x);
                        bufferWriter.Write(x);
                        break;
                    case BlockType.None:
                    default:
                        throw new InvalidEnumArgumentException(nameof(block.Type), (int)block.Type, typeof(BlockType));
                }
            }
        }

        void Parse()
        {
            var reader = new SimpleTextTemplateReader(Buffer);

            while ((reader.Read(out var value) is var type) && type != BlockType.None)
            {
                _blocks.Add((type, value));
            }
        }
    }
}