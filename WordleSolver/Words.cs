
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using Wordle.Properties;

namespace Wordle
{
    internal sealed class Words : List<Word>
    {
        internal IEnumerable<Word> ValidWords
            => this.Where(word => word.IsValid);

        internal Words() : base()
        {
            var rm = new ResourceManager("Wordle.Properties.Resources", typeof(Resources).Assembly);

            foreach (var c in Word.ALPHABETS)
            {
                var buf = (byte[])rm.GetObject(c.ToString());
                using var ms = new MemoryStream(buf);
                using var sr = new StreamReader(Stream.Synchronized(ms));
                var words = sr.ReadToEnd()
                              .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                              .Select(w => w.Trim())
                              .Select(w => (Word)w);
                AddRange(words);
            }
        } // ctor ()

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
