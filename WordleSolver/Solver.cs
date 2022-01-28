
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Diagnostics.Debug;

namespace Wordle
{
    internal static class Solver
    {
        private static readonly Words words = new();
        private static IEnumerable<int> unsealed;

        internal static event EventHandler CandidatesUpdated;

        internal static IEnumerable<Word> Candidates
        {
            get
            {
                if (CandidatesCount <= 2) return words.ValidWords;
                if (unsealed.Count() == 1) return words.ValidWords;

                return words.ValidWords.OrderWords();
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
            CandidatesUpdated?.Invoke(null, EventArgs.Empty);
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

        internal static IEnumerable<KeyValuePair<Word, int>> Regex(string pattern, bool inheritFilters = false)
        {
            var re = new Regex(pattern, RegexOptions.IgnoreCase);
            return words.Where(w => !inheritFilters | w.IsValid)
                        .Where(s => re.IsMatch((string)s))
                        .GetPriorities()
                        .OrderByDescending(kv => kv.Value);
        } // internal static IEnumerable<KeyValuePair<Word, int>> Regex (string, [bool])

        private static IEnumerable<Word> OrderWords(this IEnumerable<Word> words)
            => words.GetPriorities()
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => kv.Key);

        private static IEnumerable<KeyValuePair<Word, int>> GetPriorities(this IEnumerable<Word> words)
        {
            var cnts = new Dictionary<char, int>[Word.LENGTH];
            for (var i = 0; i < Word.LENGTH; i++)
            {
                cnts[i] = new(26);
                foreach (var c in Word.ALPHABETS)
                    cnts[i][c] = 0;
            }

            foreach (var word in words)
            {
                foreach (var index in unsealed)
                    cnts[index][word[index]] += 1;
            }

            var scores = new Dictionary<Word, int>(CandidatesCount);
            foreach (var word in words)
            {
                var s = 0;
                foreach (var index in unsealed)
                    s += cnts[index][word[index]];
                scores.Add(word, s);
            }

            return scores;
        } // private static IEnumerable<KeyValuePair<Word, int>> GetPriorities (this IEnumerable<Word>)

        internal static IEnumerable<KeyValuePair<Word, float>> NormalizePriorities(this IEnumerable<KeyValuePair<Word, int>> words)
        {
            if (!words.Any()) return Enumerable.Empty<KeyValuePair<Word, float>>();

            var vals = words.Select(kv => kv.Value);
            var min = vals.Min();
            var max = vals.Max();
            var range = max - min;

            return from item in words
                   select new KeyValuePair<Word, float>(item.Key, min != max ? (float)(item.Value - min) / range * 100 : 100f);
        } // private static IEnumerable<KeyValuePair<Word, float>> NormalizePriorities (this IEnumerable<KeyValuePair<Word, int>>)
    } // internal static class Solver
} // namespace Wordle
