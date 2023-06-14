using System;
using System.Collections.Generic;

namespace Dcis.Am.Mock.Icp.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Get the distinct list based on filter
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> filterBy)
        {
            HashSet<TKey> keys = new HashSet<TKey>();
            foreach (TSource item in source)
                if (keys.Add(filterBy(item)))
                    yield return item;
        }
    }
}
