using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 字典相关扩展
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取指定Key对应的Value,再返回由resultSelector选择出的值，或没找到则报错
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static TResult Get<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TResult> resultSelector)
        {
            TValue value = default(TValue);
            if (dict.TryGetValue(key, out value))
            {
                return resultSelector(value);
            }
            throw new KeyNotFoundException("没有找到key:" + key.ToString());
        }

        /// <summary>
        /// 获取指定Key对应的Value,再返回由resultSelector选择出的值，或没找到则返回默认值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static TResult GetOrDefault<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TResult> resultSelector)
        {
            return dict.GetOrDefault(key, resultSelector, default(TResult));
        }
        /// <summary>
        /// 获取指定Key对应的Value,再返回由resultSelector选择出的值，或没找到则返回指定默认值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="resultSelector"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult GetOrDefault<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TResult> resultSelector,TResult defaultValue)
        {
            TValue value = default(TValue);
            if (dict.TryGetValue(key, out value))
            {
                return resultSelector(value);
            }
            return defaultValue;
        }
        /// <summary>
        /// 获取指定Key对应的Value，若未找到将获取默认值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value = default(TValue);
            dict.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// 获取指定Key对应的Value，若未找到将抛异常
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value = default(TValue);
            if (!dict.TryGetValue(key, out value))
            {
                throw new KeyNotFoundException("没有找到key:" + key.ToString());
            }
            return value;
        }

        /// <summary>
        /// 获取指定Key对应的Value，若未找到将使用指定的委托增加值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> setValue)
        {
            TValue value = default(TValue);
            if (!dict.TryGetValue(key, out value))
            {
                value = setValue(key);
                dict.Add(key, value);
            }
            return value;
        }
    }
}
