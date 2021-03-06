using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace SimpleTextTemplate.Tests
{
    [SuppressMessage("Extensibility", "xUnit3001:Classes that implement Xunit.Abstractions.IXunitSerializable must have a public parameterless constructor", Justification = "static constructor")]
    public sealed class MemberSerializer<T> : IXunitSerializable
    {
        static readonly JsonSerializerOptions SerializerOptions;

        static MemberSerializer()
        {
            SerializerOptions = new();
            SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

#nullable disable
        public MemberSerializer()
        {
        }
#nullable restore

        public MemberSerializer(T obj) => Value = obj;

        public T Value { get; private set; }

        public void Deserialize(IXunitSerializationInfo info)
            => Value = JsonSerializer.Deserialize<T>(info.GetValue<string>(nameof(Value)))!;

        public void Serialize(IXunitSerializationInfo info)
            => info.AddValue(nameof(Value), JsonSerializer.Serialize(Value, SerializerOptions));

        public override string ToString() => JsonSerializer.Serialize(Value, SerializerOptions);
    }
}