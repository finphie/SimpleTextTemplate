using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace SimpleTextTemplate.Benchmarks;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public partial class RenderBenchmark
{
    const string DescriptionSimpleTextTemplate = "SimpleTextTemplate";
    const string DescriptionSimpleTextTemplateGenerator = "SimpleTextTemplate.Generator";
    const string DescriptionScriban = "Scriban";
    const string DescriptionScribanLiquid = "Liquid";
    const string DescriptionUtf8TryWrite = "(Utf8.TryWrite)";
    const string DescriptionInterpolatedStringHandler = "(InterpolatedStringHandler)";
    const string DescriptionCompositeFormat = "(CompositeFormat)";
    const string Format = "abcdef{0}abcdef";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GlobalSetup]
    public void Setup()
    {
        SetupString();
        SetupInt();
        _compositeFormat = CompositeFormat.Parse(Format);

        _generatorContext = new("_StringValue", 567890);

        _context = Context.Create();
        _context.Add("StringValue"u8.ToArray(), _generatorContext.StringValue);
        _context.Add("IntValue"u8.ToArray(), _generatorContext.IntValue);

        _scribanContext = new()
        {
            { "StringValue", _generatorContext.StringValue },
            { "IntValue", _generatorContext.IntValue }
        };
    }

    internal readonly record struct SampleContext(string StringValue, int IntValue)
    {
        public const string ConstantStringValue = "_ConstantStringValue";
        public const int ConstantIntValue = 1234;
    }
}
