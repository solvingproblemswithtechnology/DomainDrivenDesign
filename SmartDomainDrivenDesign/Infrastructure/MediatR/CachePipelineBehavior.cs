using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    public class CachePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMemoryCache cache;
        private readonly ILogger<CachePipelineBehavior<TRequest, TResponse>> logger;

        public CachePipelineBehavior(IMemoryCache cache, ILogger<CachePipelineBehavior<TRequest, TResponse>> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICacheableRequest<TResponse> cacheableRequest)
            {
                var key = cacheableRequest.GetCacheKey();

                if (cache.TryGetValue(key, out TResponse cached))
                {
                    logger.LogTrace("Cache hitted: {Key}", key);
                    return cached;
                }

                logger.LogTrace("Cache missed: {Key}", key);
            }

            TResponse response = await next().ConfigureAwait(false);

            if (request is ICacheInvalidationRequest cacheInvalidationRequest)
            {
                var key = cacheInvalidationRequest.GetCacheKey();
                cache.Remove(key);
                logger.LogTrace("Cache invalidated: {Key}", key);
            }

            return response;
        }
    }
}
