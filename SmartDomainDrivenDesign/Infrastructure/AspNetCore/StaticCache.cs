using Microsoft.Extensions.Caching.Memory;
using System;
using System.Runtime.CompilerServices;

namespace SmartDomainDrivenDesign.Infrastructure.AspNetCore
{
    public class StaticCache
    {
        private readonly IMemoryCache cache;

        public StaticCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public T Get<T>([CallerMemberName] string key = null)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            return this.cache.Get<T>(key);
        }

        public void Set<T>(T value, [CallerMemberName] string key = null)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));

            this.cache.Set(key, value);
        }
    }
}
