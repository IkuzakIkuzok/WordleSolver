
// (c) 2021 Kazuki KOHZUKI

namespace Wordle
{
    internal struct CharResult
    {
        internal int Index { get; }
        internal char Char { get; }

        internal CharResult(int index, char @char)
        {
            this.Index = index;
            this.Char = @char;
        } // ctor (int, char)

        public void Deconstruct(out int i, out char c)
        {
            i = this.Index;
            c = this.Char;
        } // public void Deconstruct (out int, out char)
    } // internal struct CharResult
} // namespace Wordle
