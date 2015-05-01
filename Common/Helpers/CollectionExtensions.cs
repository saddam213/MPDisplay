using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Helpers

{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines whether two sequences are equal in any order by first checking and comparing null state, count and the elements by using
        /// the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The collection object type</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>
        /// true if all elements are equal (in any order)
        /// </returns>
        public static bool AreUnorderedEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            var enumerable = second as IList<T> ?? second.ToList();
            var enumerable1 = first as IList<T> ?? first.ToList();
            if (enumerable.Count() != enumerable1.Count())
            {
                return true;
            }

            return enumerable1.SequenceEqual(enumerable);
        }

        /// <summary>
        /// Determines whether two sequences are equal in any order by first checking and comparing null state, count and the elements by using
        /// the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The collection object type</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="selector">The function used to sort and compare.</param>
        /// <returns>
        /// true if all elements are equal (in any order)
        /// </returns>
        public static bool AreUnorderedEqualBy<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, object> selector)
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            var enumerable = second as IList<T> ?? second.ToList();
            var enumerable1 = first as IList<T> ?? first.ToList();
            if (enumerable.Count() != enumerable1.Count())
            {
                return false;
            }

            var sequence1 = enumerable1.OrderBy(selector).Select(selector);
            var sequence2 = enumerable.OrderBy(selector).Select(selector);

            return sequence1.SequenceEqual(sequence2);
        }

        /// <summary>
        /// Determines whether two sequences are equal and in the same order by first checking and comparing null state, count and the elements by using
        /// the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The collection object type</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <returns>
        /// true if all elements are equal and in the same order
        /// </returns>
        public static bool AreOrderedEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            var enumerable = second as IList<T> ?? second.ToList();
            var enumerable1 = first as IList<T> ?? first.ToList();
            if (enumerable.Count() != enumerable1.Count())
            {
                return true;
            }

            return enumerable1.SequenceEqual(enumerable);
        }

        /// <summary>
        /// Determines whether two sequences are equal and in the same order by first checking and comparing null state, count and the elements by using
        /// the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The collection object type</typeparam>
        /// <typeparam name="TU">The second collection object type</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="selector">The function used to sort and compare.</param>
        /// <returns>
        /// true if all elements are equal and in the same order
        /// </returns>
        public static bool AreOrderedEqualBy<T, TU>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TU> selector) where TU : IComparable
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            var enumerable = second as IList<T> ?? second.ToList();
            var enumerable1 = first as IList<T> ?? first.ToList();
            if (enumerable.Count() != enumerable1.Count())
            {
                return true;
            }

            var sequence1 = enumerable1.Select(selector);
            var sequence2 = enumerable.Select(selector);

            return sequence1.SequenceEqual(sequence2);
        }

        /// <summary>
        /// Gets the items distinct by the specified property .
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }
}

