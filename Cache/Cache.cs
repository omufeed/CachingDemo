using System;
using System.Linq;
using System.Runtime.Caching;
using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace CachingDemo
{
    public enum CacheType : short
    {
        RuntimeCache = 10,
        ElastiCache = 20,
        Memcached = 30
    }

    public static class Cache
    {
        private static readonly CacheType cacheType = (CacheType)Utilities.GetAppSetting_Int("CacheType");

        public static bool Store(string key, object value)
        {
            var expireAt = DateTime.Now.AddDays(1);
            return Store(key, value, expireAt);
        }

        public static bool Store(string key, object value, DateTime expireAt)
        {
            switch (cacheType)
            {
                case CacheType.RuntimeCache:
                    return Runtime.Store(key, value, expireAt);
                case CacheType.Memcached:
                case CacheType.ElastiCache:
                    return Memcached.Store(key, value, expireAt);
                default:
                    return true;
            }
        }

        public static object Get(string key)
        {
            switch (cacheType)
            {
                case CacheType.RuntimeCache:
                    return Runtime.Get(key);
                case CacheType.Memcached:
                case CacheType.ElastiCache:
                    return Memcached.Get(key);
                default:
                    return null;
            }
        }

        public static bool Remove(string key)
        {
            switch (cacheType)
            {
                case CacheType.RuntimeCache:
                    return Runtime.Remove(key);
                case CacheType.Memcached:
                case CacheType.ElastiCache:
                    return Memcached.Remove(key);
                default:
                    return true;
            }
        }
    }

    internal static class Runtime
    {
        private static readonly ObjectCache runtimeCache = MemoryCache.Default;

        internal static bool Store(string key, object value, DateTime expireAt)
        {
            try
            {
                runtimeCache.Set(key, value, expireAt);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static object Get(string key)
        {
            try
            {
                return runtimeCache.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static bool Remove(string key)
        {
            try
            {
                runtimeCache.Remove(key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool Flush()
        {
            try
            {
                foreach (var a in (from n in runtimeCache.AsParallel() select n).ToList())
                    runtimeCache.Remove(a.Key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    internal static class Memcached
    {
        private static MemcachedClient memcachedClient;

        private static void ConnectMemcachedClient()
        {
            if (memcachedClient != null) return;

            memcachedClient = (MemcachedClient)Runtime.Get("MemcachedClient");

            if (memcachedClient != null) return;

            var cacheType = (CacheType) Utilities.GetAppSetting_Int("CahceType");

            memcachedClient = cacheType == CacheType.Memcached ? new MemcachedClient() : new MemcachedClient(new ElastiCacheClusterConfig());

            Runtime.Store("MemcachedClient", memcachedClient, DateTime.Now.AddDays(7));

            CachingDemo.Log.Debug(string.Format("New MemcachedClient; {0}", DateTime.Now));
        }

        private static void ResetMemcachedClient()
        {
            try
            {
                memcachedClient.Dispose();
            }
            catch (NullReferenceException)
            {
                //Do nothing
            }
            finally
            {
                Runtime.Remove("MemcachedClient");
                memcachedClient = null;

                ConnectMemcachedClient();
            }
        }

        internal static bool Store(string key, object value, DateTime expireAt)
        {
            try
            {
                try
                {
                    ConnectMemcachedClient();
                    if (memcachedClient.Store(StoreMode.Set, key, value, expireAt)) return true;
                }
                catch (NullReferenceException)
                {
                    ResetMemcachedClient();
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool Remove(string key)
        {
            try
            {
                ConnectMemcachedClient();
                return memcachedClient.Remove(key);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        internal static object Get(string key)
        {
            try
            {
                ConnectMemcachedClient();
                return memcachedClient.Get(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static bool Flush()
        {
            try
            {
                ConnectMemcachedClient();
                memcachedClient.FlushAll();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    internal static class Utilities
    {
        internal static string GetAppSetting(string appSettingsKey)
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings[appSettingsKey];
            }
            catch (Exception objException)
            {
                CachingDemo.Log.Error(objException);
                return null;
            }
        }

        internal static int GetAppSetting_Int(string appSettingsKey)
        {
            int keyValue;
            int.TryParse(GetAppSetting(appSettingsKey) ?? "0", out keyValue);
            return keyValue;
        }
    }
}
