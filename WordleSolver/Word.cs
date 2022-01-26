
// (c) 2021 Kazuki KOHZUKI

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Wordle
{
    internal sealed class Word : IEnumerable<char>
    {
        internal const int LENGTH = 5;
        internal const string ALPHABETS = "abcdefghijklmnopqrstuvwxyz";

        private readonly string word;

        internal char this[int index]
            => this.word[index];

        internal bool IsValid { get; set; } = true;

        internal Word(string word)
        {
            Assert(word.Length == LENGTH);
            this.word = word;
        } // ctor (string)

        internal void ApplyFilter(Filter filter)
        {
            if (!this.IsValid) return;

            foreach ((var i, var c) in filter.CorrectChars)
            {
                if (this.word[i] == c) continue;
                this.IsValid = false;
                return;
            }

            foreach ((var i, var c) in filter.IncludedChars)
            {
                var indices = this.word.AllIndices(c).Except(filter.CorrectIndices);
                if (!indices.Any())
                {
                    this.IsValid = false;
                    return;
                }

                if (this.word[i] != c) continue;
                this.IsValid = false;
                return;
            }

            foreach (var c in filter.WrongChars)
            {
                var indices = this.word.AllIndices(c).Except(filter.CorrectIndices);
                if (!indices.Any()) continue;
                this.IsValid = false;
                return;
            }
        } // internal void ApplyFilter (Filter)

        override public string ToString()
            => this.word;

        public IEnumerator<char> GetEnumerator()
            => ((IEnumerable<char>)this.word).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)this.word).GetEnumerator();

        public static explicit operator Word(string word)
            => new(word);

        public static explicit operator string(Word word)
            => word.word;
    } // internal sealed class Word : IEnumerable<char>
} // namespace Wordle
