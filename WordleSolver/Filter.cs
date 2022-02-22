
// (c) 2021 Kazuki KOHZUKI

using System.Collections.Generic;

namespace Wordle
{
    internal sealed class Filter
    {
        internal string Word { get; }
        internal ResultColors Colors { get; }

        internal IEnumerable<CharResult> CorrectChars
        {
            get
            {
                foreach ((var i, (var c, var res)) in (this.Word, this.Colors).Zip().Enumerate())
                {
                    if (res == ResultColor.Correct)
                        yield return new(i, c);
                }
            }
        }

        internal IEnumerable<int> CorrectIndices
        {
            get
            {
                foreach ((var i, var c) in this.Colors.Enumerate())
                {
                    if (c == ResultColor.Correct)
                        yield return i;
                }
            }
        }

        internal IEnumerable<CharResult> IncludedChars
        {
            get
            {
                foreach ((var i, (var c, var res)) in (this.Word, this.Colors).Zip().Enumerate())
                {
                    if (res == ResultColor.Included)
                        yield return new(i, c);
                }
            }
        }

        internal IEnumerable<char> WrongChars
        {
            get
            {
                foreach ((var c, var res) in (this.Word, this.Colors).Zip())
                {
                    if (res == ResultColor.Wrong)
                        yield return c;
                }
            }
        }

        internal double ExpectedInformation
            => new Word(this.Word).CalculateEntropy(true);

        internal Filter(string word, ResultColors colors)
        {
            this.Word = word.ToLower();
            this.Colors = colors;
        } // ctor (string, ResultColors)
    } // internal sealed class Filter
} // namespace Wordle
