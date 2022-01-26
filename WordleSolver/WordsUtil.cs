
// (c) 2021 Kazuki KOHZUKI

namespace Wordle
{
    internal static class WordsUtil
    {
        internal static int HammingDistance(Word word1, Word word2)
        {
            var d = 0;
            foreach ((var c1, var c2) in (word1, word2).Zip())
                d += c1 == c2 ? 0 : 1;

            return d;
        } // internal static int HammingDistance (string, string)
    } // internal static class WordsUtil
} // namespace Wordle
