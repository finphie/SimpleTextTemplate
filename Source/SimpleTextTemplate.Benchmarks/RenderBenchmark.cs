using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Abstractions;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    public class RenderBenchmark
    {
        IContext? _context;
        Dictionary<string, string>? _model;

        Template _template;
        ScribanTemplate? _scribanTemplate;
        ScribanTemplate? _scribanLiquidTemplate;

        [GlobalSetup]
        public void Setup()
        {
            var source = File.ReadAllBytes("Templates/Page.html");
            var sourceString = Encoding.UTF8.GetString(source);

            _template = Template.Parse(source);
            var utf8Dict = new Utf8StringDictionary<Utf8String>();
            utf8Dict.Add((Utf8String)"Identifier", (Utf8String)"Hello, World!");
            _context = Context.Create(utf8Dict);

            _scribanTemplate = ScribanTemplate.Parse(sourceString);
            _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(sourceString);
            _model = new()
            {
                { "Identifier", "Hello, World!" }
            };
        }

        [Benchmark]
        public string SimpleTextTemplate()
        {
            using var bufferWriter = new ArrayPoolBufferWriter<byte>();
            _template.RenderTo(bufferWriter, _context!);
            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

        [Benchmark(Baseline = true)]
        public string ZSimpleTextTemplate()
        {
            using var bufferWriter = new ArrayPoolBufferWriter<byte>();
            ZTemplate.GeneratePageTemplate(bufferWriter, _context);
            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

        [Benchmark]
        public byte[] ZSimpleTextTemplateUtf8()
        {
            using var bufferWriter = new ArrayPoolBufferWriter<byte>();
            ZTemplate.GeneratePageTemplate(bufferWriter, _context);
            return bufferWriter.WrittenSpan.ToArray();
        }

        [Benchmark]
        public string Scriban() => _scribanTemplate!.Render(_model);

        [Benchmark]
        public string ScribanLiquid() => _scribanLiquidTemplate!.Render(_model);
    }
}