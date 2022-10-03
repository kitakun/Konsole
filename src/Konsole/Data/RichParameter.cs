namespace Konsole.Data
{
    internal struct RichParameter
    {
        public bool IsBold;
        public bool IsItalian;
        public int FontSize;
        public string Color;

        internal static RichParameter Bold() => new() {IsBold = true};
        internal static RichParameter Italian() => new() {IsItalian = true};
    }
}