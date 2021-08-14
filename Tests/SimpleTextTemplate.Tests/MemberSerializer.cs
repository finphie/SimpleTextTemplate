using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace SimpleTextTemplate.Tests;

/// <summary>
/// xUnit.net標準で非対応のオブジェクトをシリアライズするためのクラスです。
/// </summary>
/// <typeparam name="T">シリアライズ対象の型</typeparam>
[SuppressMessage("Extensibility", "xUnit3001:Classes that implement Xunit.Abstractions.IXunitSerializable must have a public parameterless constructor", Justification = "staticコンストラクター")]
public sealed class MemberSerializer<T> : IXunitSerializable
{
    static readonly JsonSerializerOptions SerializerOptions;

    static MemberSerializer()
    {
        SerializerOptions = new();
        SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// <see cref="MemberSerializer"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="obj">オブジェクト</param>
    public MemberSerializer(T obj) => Value = obj;

    /// <summary>
    /// オブジェクトを取得します。
    /// </summary>
    /// <value>
    /// オブジェクト
    /// </value>
    public T Value { get; private set; }

    /// <inheritdoc/>
    public void Deserialize(IXunitSerializationInfo info)
        => Value = JsonSerializer.Deserialize<T>(info.GetValue<string>(nameof(Value)))!;

    /// <inheritdoc/>
    public void Serialize(IXunitSerializationInfo info)
        => info.AddValue(nameof(Value), JsonSerializer.Serialize(Value, SerializerOptions));

    /// <inheritdoc/>
    public override string ToString() => JsonSerializer.Serialize(Value, SerializerOptions);
}