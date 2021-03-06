namespace SimpleTextTemplate
{
    public readonly struct TextRange
    {
        public readonly int Start { get; }

        public readonly int End { get; }

        public int Length => End - Start;

        public TextRange(int start, int end)
            => (Start, End) = (start, end);
    }
}