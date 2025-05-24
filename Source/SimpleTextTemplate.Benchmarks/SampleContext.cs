namespace SimpleTextTemplate.Benchmarks;

sealed record SampleContext(string StringValue = "_StringValue", int IntValue = 567890)
{
    public const string ConstantStringValue = "_ConstantStringValue";
    public const int ConstantIntValue = 1234;
}
