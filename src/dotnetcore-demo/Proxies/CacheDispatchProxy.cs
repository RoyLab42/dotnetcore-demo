using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace RoyLab.DotNet.Core.Demo.Proxies
{
    public class CacheDispatchProxy<T> : DispatchProxy
        where T : class
    {
        private T backend;
        private IMemoryCache cache;
        private string cacheKeyPrefix;
        private TimeSpan cacheTimeout;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var key = $"{cacheKeyPrefix}.{targetMethod.Name}.{string.Join(".", args)}";
            if (cache.TryGetValue(key, out var result))
            {
                return result;
            }

            result = targetMethod.Invoke(backend, args);
            if (result is Task<object> task)
            {
                return task.ContinueWith((t, state) =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        var (c, k, to) = ((IMemoryCache, string, TimeSpan)) state;
                        c.Set(k, result, to);
                    }

                    return t;
                }, (cache, key, cacheTimeout)).Unwrap();
            }

            cache.Set(key, result, cacheTimeout);
            return result;
        }

        public static T Create(T backend, IMemoryCache cache, TimeSpan cacheTimeout)
        {
            object o = Create<T, CacheDispatchProxy<T>>();
            var proxy = (CacheDispatchProxy<T>) o;
            proxy.cache = cache;
            proxy.backend = backend;
            proxy.cacheKeyPrefix = backend.GetType().Name;
            proxy.cacheTimeout = cacheTimeout;
            return (T) o;
        }
    }
}