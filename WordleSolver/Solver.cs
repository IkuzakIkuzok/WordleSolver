
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Wordle
{
    internal static class Solver
    {
        private static readonly Words words = new();
        private static IEnumerable<int> unsealed;

        internal static IEnumerable<Word> Candidates
        {
            get
            {
                if (CandidatesCount <= 2) return words.ValidWords;
                if (unsealed.Count() == 1) return words.ValidWords;

                var cnts = new Dictionary<char, int>[Word.LENGTH];
                for (var i = 0; i < Word.LENGTH; i++)
                {
                    cnts[i] = new(26);
                    foreach (var c in Word.ALPHABETS)
                        cnts[i][c] = 0;
                }

                foreach (var word in words.ValidWords)
                {
                    foreach (var index in unsealed)
                        cnts[index][word[index]] += 1;
                }

                var scores = new Dictionary<Word, int>(CandidatesCount);
                foreach (var word in words.ValidWords)
                {
                    var s = 0;
                    foreach (var index in unsealed)
                        s += cnts[index][word[index]];
                    scores.Add(word, s);
                }

                return from item in scores
                       orderby item.Value
                       descending
                       select item.Key;
            }
        }

        internal static int CandidatesCount { get; private set; } = words.Count;
        internal static int SealedCount { get; private set; } = 0;

        internal static void ApplyFilter(Filter filter)
        {
            var info = Math.Log2(CandidatesCount);
            words.ApplyFilter(filter);
            CandidatesCount = words.ValidWords.Count();
            info -= Math.Log2(CandidatesCount);
            WriteLine($"Candidates: {CandidatesCount} (got {info:f4} bits of information)");
            SealedCount = filter.CorrectIndices.Count();
            unsealed = unsealed.Except(filter.CorrectIndices);
        } // internal static void ApplyFilter (Filter)

        internal static void Reset()
        {
            words.Reset();
            CandidatesCount = words.Count;
            SealedCount = 0;
            unsealed = Enumerable.Range(0, Word.LENGTH);
        } // internal static void Reset ()
    } // internal static class Solver
} // namespace Wordle
