
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Wordle
{
    internal sealed class ResultColors : IEnumerable<ResultColor>
    {
        internal static int Patterns = (int)Math.Pow(3, Word.LENGTH);
        internal static int Perfect = Patterns - 1;

        private readonly ResultColor[] colors;

        internal ResultColor this[int index]
            => this.colors[index];

        internal ResultColors(IEnumerable<ResultColor> colors)
        {
            Assert(colors.Count() == Word.LENGTH);
            this.colors = colors.ToArray();
        } // ctor (IEnumerable<ResultColor>)

        override public int GetHashCode()
        {
            var hash = 0;

            foreach (int color in this.Reverse())
            {
                hash *= 3;
                hash += color;
            }

            return hash;
        } // override public int GetHashCode ()

        public IEnumerator<ResultColor> GetEnumerator()
            => ((IEnumerable<ResultColor>)this.colors).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.colors.GetEnumerator();

        public static implicit operator ResultColors(ResultColor[] colors)
            => new(colors);
    } // internal sealed class ResultColors
} // namespace Wordle
