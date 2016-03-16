using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Svyaznoy.Core.Model;

namespace Svyaznoy.Core
{
    public static class ArrayExtensions
    {
        public static T? GetValueOrNull<T>(this IList<T> arr, int index) where T : struct
        {
            if (arr.Count > index)
            {
                return arr[index];
            }
            return null;
        }

        public static T GetValueOrNull2<T>(this IList<T> arr, int index) where T : class
        {
            if (arr.Count > index)
            {
                return arr[index];
            }
            return null;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null)
                return null;
            foreach (var item in list)
            {
                action(item);
            }
            return list;
        }

        public static IEnumerable<T> ForEachNotNull<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null)
                return null;
            foreach (var item in list)
            {
                if (item != null)
                    action(item);
            }

            return list;
        }

        public static void ForEach(this IEnumerable list, Action<Object> action)
        {
            if (list == null)
                return;
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static void ForEachNotNull(this IEnumerable list, Action<Object> action)
        {
            if (list == null)
                return;
            foreach (var item in list)
            {
                if (item != null)
                    action(item);
            }
        }

        public static void ForEach<T>(this IQueryable<T> list, Action<T> action)
        {
            if (list == null)
                return;
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static void ForEachNotNull<T>(this IQueryable<T> list, Action<T> action)
        {
            if (list == null)
                return;
            foreach (var item in list)
            {
                if (item != null)
                    action(item);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static bool HasValues<T>(this IEnumerable<T> list)
        {
            return list != null && list.Any();
        }

        public static string AsString<T>(this IEnumerable<T> list, string delimiter = ", ")
        {
            if (list == null)
                return null;

            var sb = new StringBuilder();
            int count = 0;

            list.ForEach(item =>
                {
                    if (item != null)
                    {
                        if (count > 0)
                            sb.Append(delimiter);
                        sb.Append(item.ToString());
                        count++;
                    }
                });

            return sb.ToString();
        }

        public static T RandomOrDefault<T>(this IEnumerable<T> list)
        {
            if (list != null && list.Any())
                return list.ElementAt(new Random().Next(list.Count()));
            return default(T);
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> list, int? count = null)
        {
            if (list == null)
                return new List<T>();

            var q = list.OrderBy(i => Guid.NewGuid()).AsQueryable();
            if (count.HasValue)
                q = q.Take(count.Value);
            return q.ToList();
        }

        /// <summary>
        /// Получает все возможные комбинации из входного массива значений (по 2,3...n элементов, n - длина входного массива)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> AllCombinations<T>(this IEnumerable<T> list)
        {
            var res = new List<IEnumerable<T>>();
            if (list.IsNullOrEmpty() || list.Count() <= 1)
                return res;

            var n = list.Count();
            for (int m = 2; m < n; m++)
            {
                List<T> tuple;
                if (m == 2)
                {
                    for (int i = 0; i < n - 1; i++)
                    {
                        var @base = new List<T>() {list.ElementAt(i)};
                        for (int j = i + 1; j < n; j++)
                        {
                            tuple = @base.ToList();
                            tuple.Add(list.ElementAt(j));
                            res.Add(tuple);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < n - m + 2; i++)
                    {
                        var @base = new List<T>();
                        for (int k = i; k < m + i - 1; k++)
                            @base.Add(list.ElementAt(k));

                        for (int j = m + i - 1; j < n; j++)
                        {
                            tuple = @base.ToList();
                            tuple.Add(list.ElementAt(j));
                            res.Add(tuple);
                        }
                        for (int j = 0; j < i - 1; j++)
                        {
                            tuple = @base.ToList();
                            tuple.Add(list.ElementAt(j));
                            res.Add(tuple);
                        }
                    }
                }
            }

            res.Add(list.ToList());

            return res;
        }

        public static IQueryable<TSource> OrderByCascade<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector, OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Ascending:
                    return source.OrderByCascade(keySelector);
                    case OrderType.Descending:
                    return source.OrderByDescendingCascade(keySelector);
            }
            return source;
        }

        public static IQueryable<TSource> OrderByCascade<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector)
        {
            var isOrderedSource = typeof(IOrderedQueryable<TSource>).IsAssignableFrom(source.Expression.Type);
            var orderedSource = source as IOrderedQueryable<TSource>;

            if (isOrderedSource && orderedSource!=null)
            {
                return orderedSource.ThenBy(keySelector);
            }
            else
            {
                return source.OrderBy(keySelector);
            }
        }

        public static IQueryable<TSource> OrderByDescendingCascade<TSource, TKey>(this IQueryable<TSource> source,
                                                                    Expression<Func<TSource, TKey>> keySelector)
        {
            var isOrderedSource = typeof(IOrderedQueryable<TSource>).IsAssignableFrom(source.Expression.Type);
            var orderedSource = source as IOrderedQueryable<TSource>;

            if (isOrderedSource && orderedSource != null)
            {
                return orderedSource.ThenByDescending(keySelector);
            }
            else
            {
                return source.OrderByDescending(keySelector);
            }
        }


        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        } 
    }
}