﻿namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// 使用するGrowメソッドの種類を表します。
/// </summary>
/// <param name="ConstantCount">定数書き込みの際のバイト数</param>
/// <param name="Members">メンバーのリスト</param>
sealed record TemplateWriterGrowInfo(
    int ConstantCount,
    IReadOnlyList<ContextMember> Members);
