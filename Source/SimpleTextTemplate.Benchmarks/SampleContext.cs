namespace SimpleTextTemplate.Benchmarks;

sealed record SampleContext(string StringValue = "zyxwv", int IntValue = 67890)
{
    public const string ConstantStringValue = "abcde";
    public const int ConstantIntValue = 12345;
}
