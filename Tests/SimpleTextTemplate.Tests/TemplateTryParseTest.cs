using System.Text;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateTryParseTest
{
    [Test]
    [Arguments("")]
    [Arguments("{")]
    [Arguments("a")]
    [Arguments("ab")]
    [Arguments("abc")]
    [Arguments("{ A }")]
    [Arguments("}}")]
    public Task 文字列_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ A }}{{ B }}")]
    [Arguments("{{ AAA }}{{ BBB }}")]
    public Task 識別子_識別子_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("z{{A}}z")]
    [Arguments("z{{ A }}z")]
    public Task 文字列_識別子_文字列_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ AAA }}z{{ BBB }}")]
    [Arguments("{{ A }}123{{ B }}")]
    public Task 識別子_文字列_識別子_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("x{{ A }}123{{ B }}x")]
    public Task 文字列_識別子_文字列_識別子_文字列_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ 1::ja-JP }}")]
    [Arguments("{{ 1:N:ja-JP }}")]
    public Task カルチャー指定あり_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ A: }}")]
    [Arguments("{{ A:: }}")]
    [Arguments("{{ A:N: }}")]
    public Task 書式指定またはカルチャー指定が空_trueを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{", 2)]
    [Arguments("{{ ", 3)]
    [Arguments("{{ }", 3)]
    public async Task 識別子終了タグなし_falseを返す(string input, int expectedConsumed)
    {
        var result = Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed);

        await Assert.That(result).IsFalse();
        await Assert.That(consumed).IsEqualTo((nuint)expectedConsumed);
    }

    [Test]
    [Arguments("{{ : }}")]
    [Arguments("{{ :: }}")]
    public async Task 識別子が空_falseを返す(string input)
    {
        var result = Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed);

        await Assert.That(result).IsFalse();
        await Assert.That(consumed).IsEqualTo((nuint)input.Length);
    }

    [Test]
    public async Task 無効なカルチャー_falseを返す()
    {
        var input = "{{ A::B }}"u8.ToArray();
        var result = Template.TryParse(input, out _, out var consumed);

        await Assert.That(result).IsFalse();
        await Assert.That(consumed).IsEqualTo((nuint)input.Length);
    }

    static async Task Execute(string input)
    {
        var result = Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed);

        await Assert.That(result).IsTrue();
        await Assert.That(consumed).IsEqualTo((nuint)input.Length);
    }
}
