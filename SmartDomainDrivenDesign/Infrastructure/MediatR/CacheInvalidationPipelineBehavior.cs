using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    public class CacheInvalidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheInvalidationRequest
    {
        private readonly IMemoryCache cache;
        private readonly ILogger<CacheInvalidationPipelineBehavior<TRequest, TResponse>> logger;

        public CacheInvalidationPipelineBehavior(IMemoryCache cache, ILogger<CacheInvalidationPipelineBehavior<TRequest, TResponse>> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = await next().ConfigureAwait(false);

            string key = request.GetCacheKey();
            cache.Remove(key);
            logger.LogTrace("Cache invalidated: {Key}", key);

            return response;
        }
    }
}
