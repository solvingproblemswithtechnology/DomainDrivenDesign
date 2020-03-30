using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.AspNetCore
{
    public static class SitePipelines
    {
        /// <summary>
        /// Añade los ProblemDetails por defecto 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IEndpointConventionBuilder MapSmartHealthChecks(this IEndpointRouteBuilder endpoints) => endpoints.MapHealthChecks("/healthchecks", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        /// <summary>
        /// Añade los ProblemDetails por defecto 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IApplicationBuilder UseSmartSerilogRequestLogging(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var request = httpContext.Request;

                    diagnosticContext.Set("Host", request.Host);
                    diagnosticContext.Set("Protocol", request.Protocol);
                    diagnosticContext.Set("Scheme", request.Scheme);

                    if (request.QueryString.HasValue)
                        diagnosticContext.Set("QueryString", request.QueryString.Value);

                    diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

                    var endpoint = httpContext.GetEndpoint();
                    if (endpoint is object) // endpoint != null
                    {
                        diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                    }

                    diagnosticContext.Set("Usuario", httpContext.User?.Identity?.Name ?? "Anónimo");
                };

                options.GetLevel = (HttpContext ctx, double _, Exception ex) =>
                    ex != null
                        ? LogEventLevel.Error
                        : ctx.Response.StatusCode > 499
                            ? LogEventLevel.Error
                            : ctx.GetEndpoint()?.DisplayName?.Equals("Health checks", StringComparison.Ordinal) == true
                                ? LogEventLevel.Verbose
                                : LogEventLevel.Information;
            });
        }

        /// <summary>
        /// Añade los ProblemDetails por defecto 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IApplicationBuilder UseSmartLoggingMiddleware(this IApplicationBuilder app) => app.Use(async (ctx, next) =>
        {
            LogContext.PushProperty("Usuario", ctx.GetUsername());
            await next();
        });

        /// <summary>
        /// Arranca el contexto de base de datos. No es necesario hacer Await.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static async Task WarmupEntityFrameworkAsync<T>(this IApplicationBuilder app) where T : DbContext
        {
            using var scope = app.ApplicationServices.CreateScope();
            await scope.ServiceProvider.GetService<T>().Database.OpenConnectionAsync();
        }
    }
}
