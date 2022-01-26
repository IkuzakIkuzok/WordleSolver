
// (c) 2021 Kazuki KOHZUKI

using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Wordle
{
    internal sealed class Filter
    {
        internal string Word { get; }
        internal ResultColor[] Colors { get; }

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

        internal Filter(string word, ResultColor[] colors)
        {
            Assert(colors.Length == Wordle.Word.LENGTH);

            this.Word = word.ToLower();
            this.Colors = colors;
        } // ctor (string, ResultColor[])
    } // internal sealed class Filter
} // namespace Wordle
