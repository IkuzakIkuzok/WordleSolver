
// (c) 2021 Kazuki KOHZUKI

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Diagnostics.Debug;
using static System.Linq.Enumerable;

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
            var res = GetResults(filter.Word);
            this.IsValid = res == filter.Colors;
        } // internal void ApplyFilter (Filter)

        internal ResultColors GetResults(string pattern)
        {
            var res = new ResultColor[5];
            var word = this.word;

            foreach (var i in Range(0, 5))
            {
                if (pattern[i] == word[i])
                {
                    res[i] = ResultColor.Correct;
                    word = word.Remove(i, 1).Insert(i, "*");
                }
            }

            foreach ((var i, var c) in pattern.Enumerate())
            {
                if (word.Contains(c) && res[i] == ResultColor.Wrong)
                {
                    res[i] = ResultColor.Included;
                    var index = word.IndexOf(c);
                    word = word.Remove(index, 1).Insert(i, "*");
                }
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
