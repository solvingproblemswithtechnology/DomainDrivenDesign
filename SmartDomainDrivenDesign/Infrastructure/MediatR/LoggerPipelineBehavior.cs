using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
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
            logger.LogTrace("Starting command: " + requestName); // Just example

            var response = await next();

            logger.LogTrace("Ended command: " + requestName); // Just example

            return response;
        }
    }
}
