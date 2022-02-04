
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Wordle.Properties;

namespace Wordle
{
    internal sealed class Words : List<Word>
    {
        internal IEnumerable<Word> ValidWords
            => this.Where(word => word.IsValid);

        internal Words(WordListType listType) : base()
        {
            var rm = new ResourceManager("Wordle.Properties.Resources", typeof(Resources).Assembly);

            var file = listType.ToString().ToLower();
            var words = rm.GetObject(file)
                          .ToString()
                          .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                          .Select(w => w.Trim())
                          .Select(w => (Word)w);
            AddRange(words);
        } // ctor (WordListType)

        internal void ApplyFilter(Filter filter)
        {
            foreach (var word in this.ValidWords)
                word.ApplyFilter(filter);
        } // internal void ApplyFilter (Filter)

        internal void Reset()
        {
            foreach (var word in this)
                word.IsValid = true;
        } // internal void Reset ()
    } // internal sealed class Words : List<Word>
} // namespace Wordle
