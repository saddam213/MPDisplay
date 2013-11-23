using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
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

            if (second.Count() != first.Count())
            {
                return true;
            }

            var sequence1 = first.OrderBy(x => x);
            var sequence2 = second.OrderBy(x => x);

            return first.SequenceEqual(second);
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

            if (second.Count() != first.Count())
            {
                return false;
            }

            var sequence1 = first.OrderBy(selector).Select(selector);
            var sequence2 = second.OrderBy(selector).Select(selector);

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

            if (second.Count() != first.Count())
            {
                return true;
            }

            return first.SequenceEqual(second);
        }

        /// <summary>
        /// Determines whether two sequences are equal and in the same order by first checking and comparing null state, count and the elements by using
        /// the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The collection object type</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="selector">The function used to sort and compare.</param>
        /// <returns>
        /// true if all elements are equal and in the same order
        /// </returns>
        public static bool AreOrderedEqualBy<T, U>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, U> selector) where U : IComparable
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            if (second.Count() != first.Count())
            {
                return true;
            }

            var sequence1 = first.Select(selector);
            var sequence2 = second.Select(selector);

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
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

