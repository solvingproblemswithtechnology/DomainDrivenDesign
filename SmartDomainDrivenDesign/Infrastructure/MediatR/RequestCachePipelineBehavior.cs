using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    public class RequestCachePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableRequest<TResponse>
    {
        private readonly IMemoryCache cache;
        private readonly ILogger<RequestCachePipelineBehavior<TRequest, TResponse>> logger;

        public RequestCachePipelineBehavior(IMemoryCache cache, ILogger<RequestCachePipelineBehavior<TRequest, TResponse>> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            object key = request.GetCacheKey();

            if (cache.TryGetValue(key, out TResponse cached))
            {
                logger.LogTrace("Cache hitted: {Key}", key);
                return cached;
            }

            logger.LogTrace("Cache missed: {Key}", key);

            TResponse response = await next().ConfigureAwait(false);

            return response;
        }
    }
}
