using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using SimpleTextTemplate.Contexts;
using Utf8Utility;

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
    IContext _context;
    Dictionary<string, object> _scribanContext;

    [GlobalSetup]
    public void Setup()
    {
        SetupString();
        SetupInt();
        _compositeFormat = CompositeFormat.Parse(Format);

        _generatorContext = new("_StringValue", 567890);

        var utf8Dict = new Utf8ArrayDictionary<object>();
        utf8Dict.TryAdd(new("StringValue"), _generatorContext.StringValue);
        utf8Dict.TryAdd(new("IntValue"), _generatorContext.IntValue);

        _context = Context.Create(utf8Dict);

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
