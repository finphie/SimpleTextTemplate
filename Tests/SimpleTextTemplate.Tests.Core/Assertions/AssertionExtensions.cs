using System.Text;
using TUnit.Assertions.Conditions;
using TUnit.Assertions.Core;
using TUnit.Assertions.Enums;

namespace SimpleTextTemplate.Tests.Assertions;

public static class AssertionExtensions
{
    extension(IAssertionSource<ReadOnlyMemory<byte>> source)
    {
        public IsEquivalentToAssertion<char[], char> IsUtf8SequenceEqualTo(string? expected)
        {
            return new IsEquivalentToAssertion<char[], char>(
                source.Context.Map(static x => Encoding.UTF8.GetChars(x.Span.ToArray())),
                expected?.ToCharArray() ?? [],
                CollectionOrdering.Matching);
        }
    }
}
