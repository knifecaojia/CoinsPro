using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
namespace CacheRedis
{
    using Common;
    using NHibernate.Criterion;
    using ServiceStack.Redis;
    using ServiceStack.Redis.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>  
    /// Redis公共辅助类库  
    /// </summary>  
    public class RedisHelper : IDisposable
    {

        private static RedisClient Redis;

        //缓存池  
        private static PooledRedisClientManager prcm = new PooledRedisClientManager();

        //默认缓存过期时间单位秒  
        public int secondsTimeOut = 30 * 60;

        /// <summary>  
        /// 缓冲池  
        /// </summary>  
        /// <param name="readWriteHosts"></param>  
        /// <param name="readOnlyHosts"></param>  
        /// <returns></returns>  
        public static PooledRedisClientManager CreateManager(
         string[] readWriteHosts, string[] readOnlyHosts)
        {

            prcm = new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
                new RedisClientManagerConfig
                {
                    MaxWritePoolSize = readWriteHosts.Length * 5,
                    MaxReadPoolSize = readOnlyHosts.Length * 5,
                    AutoStart = true,
                });
            return prcm;
        }

        RedisHelper()
        {
            Redis = Redis ?? (Redis = new RedisClient("127.0.0.1", 6379, "123456"));
        }
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="openPooledRedis">是否开启缓冲池</param>  
        public RedisHelper(bool openPooledRedis = false)
        {
            if (openPooledRedis)
            {
                 Redis = Redis ?? (Redis = GetClient());
            }
        }

        public static RedisClient GetClient()
        {
            prcm = CreateManager(new string[] { "123456@127.0.0.1:6379" }, new string[] { "123456@127.0.0.1:6380" });
            Redis = prcm.GetClient() as RedisClient;
            return Redis;
        }


        public static bool Ping()
        {
            try
            {
                if (GetClient().Ping())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public int Exist(string key)
        {
            try
            {
                return Redis.Exists(key);
            }
            catch
            {

                return 0;
            }
        }
        #region Key/Value存储
        /// <summary>  
        /// 设置缓存  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="key">缓存建</param>  
        /// <param name="t">缓存值</param>  
        /// <param name="timeout">过期时间，单位秒,-1：不过期，0：默认过期时间</param>  
        /// <returns></returns>  
        public bool Set<T>(string key, T t, int timeout = 0)
        {
            try
            {
            if (timeout >= 0)
            {
                if (timeout > 0)
                {
                    secondsTimeOut = timeout;
                }
                Redis.Expire(key, secondsTimeOut);
            }

            return Redis.Set<T>(key, t);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>  
        /// 获取  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public T Get<T>(string key)
        {
            try
            {
                return Redis.Get<T>(key);
            }
            catch 
            {

                return default(T);
            }
        }
        /// <summary>  
        /// 删除  
        /// </summary>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public bool Remove(string key)
        {
            try
            {
                return Redis.Remove(key);
            }
            catch
            {
                return false;
            }
        }

        public bool Add<T>(string key, T t, int timeout)
        {
            try
            {
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(key, secondsTimeOut);
                }
                return Redis.Add<T>(key, t);
            }
            catch 
            {

                return false;
            }

        }
        #endregion

        #region 链表操作
        /// <summary>  
        /// 根据IEnumerable数据添加链表  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="listId"></param>  
        /// <param name="values"></param>  
        /// <param name="timeout"></param>  
        public void AddList<T>(string listId, IEnumerable<T> values, int timeout = 0)
        {
            try
            {
                Redis.Expire(listId, 60);
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(listId, secondsTimeOut);
                }
                var redisList = iredisClient.Lists[listId];
                redisList.AddRange(values);
                iredisClient.Save();
            }
            catch 
            {

            }

        }
        /// <summary>  
        /// 添加单个实体到链表中  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="listId"></param>  
        /// <param name="Item"></param>  
        /// <param name="timeout"></param>  
        public void AddEntityToList<T>(string listId, T Item, int timeout = 0)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        secondsTimeOut = timeout;
                    }
                    Redis.Expire(listId, secondsTimeOut);
                }
                var redisList = iredisClient.Lists[listId];
                redisList.Add(Item);
                iredisClient.Save();
            }
            catch 
            {
                
            }
        }
        /// <summary>  
        /// 获取链表  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="listId"></param>  
        /// <returns></returns>  
        public IEnumerable<T> GetList<T>(string listId)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                return iredisClient.Lists[listId];
            }
            catch 
            {

                return null;
            }
        }
        /// <summary>  
        /// 在链表中删除单个实体  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="listId"></param>  
        /// <param name="t"></param>  
        public void RemoveEntityFromList<T>(string listId, T t)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = Redis.As<T>();
                var redisList = iredisClient.Lists[listId];
                redisList.RemoveValue(t);
                iredisClient.Save();
            }
            catch
            {
            }
        }
        /// <summary>  
        /// 根据lambada表达式删除符合条件的实体  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="listId"></param>  
        /// <param name="func"></param>  
        public void RemoveEntityFromList<T>(string listId, Func<T, bool> func)
        {
            try
            {
                using (IRedisTypedClient<T> iredisClient = Redis.As<T>())
                {
                    var redisList = iredisClient.Lists[listId];
                    T value = redisList.Where(func).FirstOrDefault();
                    redisList.RemoveValue(value);
                    iredisClient.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 删除关联实体
        /// </summary>
        /// <param name="key"></param>
        public void DelJoin(string key)
        {
            try
            {
                //检查是否存在关联
                if (Exist(key) > 0)
                {
                    string[] keys = Get<string[]>(key);
                    foreach (var item in keys)
                    {
                        Remove(item);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 添加关联实体
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddJoin(string key, string value)
        {
            try
            {
                if (Exist(key) > 0) //存在
                {
                    string[] keys = Get<string[]>(key);
                    if (!keys.Contains(value))
                    {
                        List<string> list = keys.ToList<string>();
                        list.Add(value);
                        Set<string[]>(key, list.ToArray());
                    }
                }
                else
                {
                    Set<string[]>(key, new string[] { value });
                }
            }
            catch
            {
            }
        }
        #endregion
        //释放资源  
        public void Dispose()
        {
            if (Redis != null)
            {
                Redis.Dispose();
                Redis = null;
            }
            GC.Collect();
        }

        public string GetString<T>(List<T> t) 
        {
            if (t == null) return "";
            return JsonConvert.SerializeObject(t);
            //byte[] by = Encrypt.SerializeObject_ToByte(t);
            //return Common.Encrypt.md5(Convert.ToBase64String(by));
        }
        public string GetString(List<Order> order)
        {
            var res = "";
            if (order == null) return res;
            foreach (var item in order)
            {
                res += item.ToString();
            }
            return res;
        }

        //public static string GetJson<T>(List<T> obj)
        //{
        //    //记住 添加引用 System.ServiceModel.Web 
        //    /**
        //     * 如果不添加上面的引用,System.Runtime.Serialization.Json; Json是出不来的哦
        //     * */
        //    DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        json.WriteObject(ms, obj);
        //        string szJson = Encoding.UTF8.GetString(ms.ToArray());
        //        return szJson;
        //    }
        //}


    }
}