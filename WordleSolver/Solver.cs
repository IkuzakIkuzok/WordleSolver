
// (c) 2021 Kazuki KOHZUKI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Diagnostics.Debug;
using WordData = System.Collections.Generic.KeyValuePair<Wordle.Word, double>;
using WordsData = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Wordle.Word, double>>;

namespace Wordle
{
    internal static class Solver
    {
        private static readonly Words candidates = new(WordListType.Candidates);
        private static readonly Words valids = new(WordListType.Valids);
        private static IEnumerable<int> unsealed;
        private static HashSet<char> wrongs;
        private static SolverMode mode = SolverMode.HardMode;
        private static Func<IEnumerable<Word>, WordsData> scoresGetter = GetPriorities;

        private static readonly DateTime firstDay = new(2021, 6, 19, 0, 0, 0);

        internal static bool fireEvent = true;
        internal static event EventHandler CandidatesUpdated;
        internal static event EventHandler AlgorithmChanged;

        internal static SolverMode SolverMode
        {
            get => mode;
            set
            {
                mode = value;
                scoresGetter = ((mode & SolverMode.UseEntropy) == SolverMode.UseEntropy) ? CalculateEntropies : GetPriorities;
                if (fireEvent)
                    AlgorithmChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        internal static bool UseEntropy => (SolverMode & SolverMode.UseEntropy) == SolverMode.UseEntropy;

        internal static bool HardMode => (SolverMode & SolverMode.NonHardMode) != SolverMode.NonHardMode;

        internal static IEnumerable<Word> Candidates
        {
            get
            {
                if (CandidatesCount <= 2) return candidates.ValidWords;
                if (HardMode && unsealed.Count() == 1) return candidates.ValidWords;

                return Words.Where(w => w.IsValid).OrderWords();
            }
        }

        internal static IEnumerable<Word> Words => HardMode ? candidates : candidates.Concat(valids);
        internal static IEnumerable<Word> CandidateWords => candidates;

        internal static int CandidatesCount { get; private set; } = candidates.Count;
        internal static int AllCandidatesCount => candidates.Count;
        internal static int SealedCount { get; private set; } = 0;

        internal static void ApplyFilter(Filter filter)
        {
            var info = Math.Log2(CandidatesCount);
            var entropy = filter.ExpectedInformation;
            candidates.ApplyFilter(filter);
            valids.ApplyFilter(filter);
            CandidatesCount = candidates.ValidWords.Count();
            info -= Math.Log2(CandidatesCount);
            WriteLine($"Candidates: {CandidatesCount} word(s) (expected {entropy:f4} bits of information, got {info:f4} bits)");
            CandidatesUpdated?.Invoke(null, EventArgs.Empty);
            SealedCount = filter.CorrectIndices.Count();
            unsealed = unsealed.Except(filter.CorrectIndices);
            foreach (var c in filter.WrongChars)
                wrongs.Add(c);
        } // internal static void ApplyFilter (Filter)

        internal static void Reset()
        {
            candidates.Reset();
            valids.Reset();
            CandidatesCount = candidates.Count;
            SealedCount = 0;
            unsealed = Enumerable.Range(0, Word.LENGTH);
            wrongs = new();
            CandidatesUpdated?.Invoke(null, EventArgs.Empty);
        } // internal static void Reset ()

        internal static WordsData Regex(string pattern, bool includeValids, out bool succeeded, bool inheritFilters = false)
        {
            var oldMode = SolverMode;
            fireEvent = false;
            SolverMode &= SolverMode.UseEntropy;
            try
            {
                succeeded = true;

                var words = includeValids ? candidates.Concat(valids) : candidates;
                words = words.Where(w => !inheritFilters | w.IsValid);
                try
                {
                    var re = new Regex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1));
                    words = words.Where(s => re.IsMatch(s));
                }
                catch
                {
                    succeeded = false;
                    return Enumerable.Empty<WordData>();
                }

                return scoresGetter(words).OrderByDescending(kv => kv.Value);
            }
            finally
            {
                SolverMode = oldMode;
                fireEvent = true;
            }
            
        } // internal static WordsData Regex (string, out bool, [bool])

        private static IEnumerable<Word> OrderWords(this IEnumerable<Word> words)
            => scoresGetter(words).OrderByDescending(kv => kv.Value)
                                  .ToList()
                                  .Select(kv => kv.Key);

        private static WordsData GetPriorities(this IEnumerable<Word> words)
        {
            var cnts = new Dictionary<char, int>[Word.LENGTH];
            for (var i = 0; i < Word.LENGTH; i++)
            {
                cnts[i] = new(Word.ALPHABETS.Length);
                foreach (var c in Word.ALPHABETS)
                    cnts[i][c] = 0;
            }

            if (HardMode)
            {
                foreach (var word in words)
                {
                    foreach (var index in unsealed)
                        cnts[index][word[index]] += 1;
                }
            }
            else
            {
                foreach (var word in words)
                {
                    foreach (var index in unsealed)
                    {
                        foreach (var cnt in cnts)
                            cnt[word[index]] += 1;
                    }
                }
                foreach (var c in wrongs)
                {
                    foreach (var cnt in cnts)
                        cnt[c] -= 1;
                }
            }
            

            var scores = new Dictionary<Word, double>(CandidatesCount);
            var indices = HardMode ? unsealed : Enumerable.Range(0, Word.LENGTH);
            foreach (var word in HardMode ? words : candidates)
            {
                var s = 0;
                foreach (var index in indices)
                    s += cnts[index][word[index]];
                scores.Add(word, s);
            }


            foreach (var word in scores.Keys)
                scores[word] *= word.Distinct().Count();

            return scores;
        } // private static WordsData GetPriorities (this IEnumerable<Word>)

        internal static WordsData NormalizePriorities(this WordsData words)
        {
            if (!words.Any()) return Enumerable.Empty<WordData>();

            var vals = words.Select(kv => kv.Value);
            var min = vals.Min();
            var max = vals.Max();
            var range = max - min;

            return from item in words
                   select new WordData(item.Key, min != max ? (double)(item.Value - min) / range * 100 : 100f);
        } // private static WordsData NormalizePriorities (this WordsData)

        internal static WordsData CalculateEntropies(this IEnumerable<Word> words)
        {
            if (HardMode)
                return words.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                            .Select(w => new WordData(w, w.CalculateEntropy(words)));
            else
            {
                const double WEIGHT = 1;
                var entropies = candidates.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                                          .Select(w => new WordData(w, w.CalculateEntropy(words)))
                                          .ToDictionary(x => x.Key, x => x.Value);
                var weights = words.GetPriorities();
                return weights.Select(w => new WordData(w.Key, w.Value * WEIGHT / 100 / WEIGHT * entropies[w.Key]));
            }
        }

        internal static double CalculateEntropy(this Word word, bool inheritFilters = false)
        {
            var words = candidates.Where(w => !inheritFilters | w.IsValid);
            return word.CalculateEntropy(words);
        } // internal static double CalculateEntropy (this Word, [bool])

        internal static double CalculateEntropy(this Word word, IEnumerable<Word> words)
        {
            var cnt = words.Count();
            var cnts = new int[ResultColors.Patterns];
            foreach (var w in words)
                cnts[w.GetResults(word).GetHashCode()] += 1;

            return cnts.Select(c => (double)c / cnt)
                       .Select(p => p > 0 ? -p * Math.Log2(p) : 0)
                       .Sum();
        } // internal static double CalculateEntropy (this Word, IEnumerable<Word>)

        internal static int GetId(Word word)
            => candidates.IndexOf(word);

        internal static int GetId(DateTime date)
            => (date - firstDay).Days % candidates.Count;

        internal static Word GetAnswer(int id)
            => candidates[id];
    } // internal static class Solver
} // namespace Wordle
