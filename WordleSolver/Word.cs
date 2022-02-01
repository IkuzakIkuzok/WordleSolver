
// (c) 2021 Kazuki KOHZUKI

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            this.word = word.ToLower();
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

        internal ResultColors GetResults(string pattern)
        {
            var res = new ResultColor[5];

            var corrects = new Dictionary<char, int>(26);
            foreach (var c in ALPHABETS)
                corrects[c] = 0;
            var sb_remain = new StringBuilder();
            foreach ((var i, var c) in pattern.Enumerate())
            {
                if (this.word[i] == c)
                    res[i] = ResultColor.Correct;
                else
                    sb_remain.Append(this.word[i]);
            }

            var remain = sb_remain.ToString();
            foreach ((var i, var c) in pattern.Enumerate())
            {
                if (res[i] == ResultColor.Correct) continue;
                if (remain.Contains(c))
                    res[i] = ResultColor.Included;
            }

            return res;
        } // internal ResultColors GetResults (string)

        override public string ToString()
            => this.word;

        override public bool Equals(object obj)
        {
            if (obj is not Word word) return false;
            return this.word == word.word;
        } // override public bool Equals (object)

        override public int GetHashCode()
            => this.word.GetHashCode();

        public IEnumerator<char> GetEnumerator()
            => ((IEnumerable<char>)this.word).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)this.word).GetEnumerator();

        public static explicit operator Word(string word)
            => new(word);

        public static implicit operator string(Word word)
            => word.word;
    } // internal sealed class Word : IEnumerable<char>
} // namespace Wordle
