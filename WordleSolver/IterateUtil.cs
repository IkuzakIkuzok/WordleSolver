
// (c) 2021 Kazuki KOHZUKI

using System.Collections.Generic;

namespace Wordle
{
    internal static class IterateUtil
    {
        internal static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> @this)
        {
            var index = 0;
            foreach (var item in @this)
                yield return (index++, item);
        } // internal static IEnumerable<(int, T)> Enumerate<T> (this IEnumerable<T>)

        internal static IEnumerable<(T1, T2)> Zip<T1, T2>(this (IEnumerable<T1>, IEnumerable<T2>) @this)
        {
            var iter1 = @this.Item1.GetEnumerator();
            var iter2 = @this.Item2.GetEnumerator();
            while (iter1.MoveNext() && iter2.MoveNext())
                yield return (iter1.Current, iter2.Current);
        } // internal static IEnumerable<(T1, T2)> Zip<T1, T2> (this (IEnumerable<T1>, IEnumerable<T2>))

        internal static IEnumerable<int> AllIndices<T>(this IEnumerable<T> @this, T target)
        {
            foreach ((var i, var item) in @this.Enumerate())
            {
                if (item.Equals(target))
                    yield return i;
            }
        } // internal static IEnumerable<int> AllIndices<T> (this IEnumerable<T>, T)
    } // internal static class IterateUtil
} // namespace Wordle
