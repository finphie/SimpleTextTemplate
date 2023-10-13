using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace SimpleTextTemplate.Tests;

/// <summary>
/// xUnit.net標準で非対応のオブジェクトをシリアライズするためのクラスです。
/// </summary>
/// <typeparam name="T">シリアライズ対象の型</typeparam>
/// <param name="obj">オブジェクト</param>
[SuppressMessage("Extensibility", "xUnit3001:Classes that implement Xunit.Abstractions.IXunitSerializable must have a public parameterless constructor", Justification = "staticコンストラクター")]
public sealed class MemberSerializer<T>(T obj) : IXunitSerializable
{
    static readonly JsonSerializerOptions SerializerOptions;

    static MemberSerializer()
    {
        SerializerOptions = new();
        SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// オブジェクトを取得します。
    /// </summary>
    /// <value>
    /// オブジェクト
    /// </value>
    public T Value { get; private set; } = obj;

    /// <inheritdoc/>
    public void Deserialize(IXunitSerializationInfo info)
        => Value = JsonSerializer.Deserialize<T>(info.GetValue<string>(nameof(Value)))!;

    /// <inheritdoc/>
    public void Serialize(IXunitSerializationInfo info)
        => info.AddValue(nameof(Value), JsonSerializer.Serialize(Value, SerializerOptions));

    /// <inheritdoc/>
    public override string ToString() => JsonSerializer.Serialize(Value, SerializerOptions);
}
