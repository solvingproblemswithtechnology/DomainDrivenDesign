using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    public class CacheableRequestPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableRequest<TResponse>
    {
        private readonly IMemoryCache cache;
        private readonly ILogger<CacheableRequestPipelineBehavior<TRequest, TResponse>> logger;

        public CacheableRequestPipelineBehavior(IMemoryCache cache, ILogger<CacheableRequestPipelineBehavior<TRequest, TResponse>> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            object key = request.GetCacheKey();

            if (this.cache.TryGetValue(key, out TResponse cached))
            {
                this.logger.LogTrace("Cache hitted: {Key}", key);
                return cached;
            }

            this.logger.LogTrace("Cache missed: {Key}", key);

            return await next().ConfigureAwait(false);
        }
    }
}
