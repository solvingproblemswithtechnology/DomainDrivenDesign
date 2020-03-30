using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    /// <summary>
    /// Adds logging at the start and the end of a request with Tracing info
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private static readonly string requestName = typeof(TRequest).Name;
        private readonly ILogger<LoggerPipelineBehavior<TRequest, TResponse>> logger;

        public LoggerPipelineBehavior(ILogger<LoggerPipelineBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            logger.LogTrace("Starting command: " + requestName);

            TResponse response = await next().ConfigureAwait(false);

            logger.LogTrace("Ended command: " + requestName);

            return response;
        }
    }
}
