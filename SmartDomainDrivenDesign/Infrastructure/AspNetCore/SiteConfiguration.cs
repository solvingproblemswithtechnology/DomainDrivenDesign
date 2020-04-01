using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SmartDomainDrivenDesign.Infrastructure.EntityFrameworkCore;
using System;
using System.IO.Compression;
using System.Net.Http;

namespace SmartDomainDrivenDesign.Infrastructure.AspNetCore
{
    public static class SiteConfiguration
    {
        /// <summary>
        /// Añade Brotli y Gzip como proveedores de compresión
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSmartCompression(this IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            return services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = new string[]
                {
                    "text/plain", "text/css", "application/javascript", "text/html",
                    "application/xml", "text/xml", "image/svg+xml", "image/png",
                    "image/jpg", "image/jpeg", "text/csv", "application/json"
                };
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            });
        }

        /// <summary>
        /// Añade swagger con la configuración por defecto
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        /// <param name="version"></param>
        //public static IServiceCollection AddSmartSwaggerGen(this IServiceCollection services, Action<AspNetCoreOpenApiDocumentGeneratorSettings, IServiceProvider> configure = null)
        //{
        //    return services.AddOpenApiDocument((options, services) =>
        //    {
        //        options.DocumentName = "Web Api";
        //        options.Version = "v1";
        //        options.Description = "Web Api description";
        //        options.PostProcess = document =>
        //        {
        //            document.Info.Version = "v1";
        //            document.Info.Title = "Web Api";
        //            document.Info.Description = "Web Api description";
        //            document.Info.Contact = new NSwag.OpenApiContact
        //            {
        //                Name = "",
        //                Email = "",
        //                Url = ""
        //            };
        //        };

        //        configure?.Invoke(options, services);
        //    });
        //}

        /// <summary>
        /// Añade los controladores, configura newtonsoft y 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IServiceCollection AddSmartControllers(this IServiceCollection services, IHostEnvironment environment)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DefaultValueHandling = environment.IsDevelopment() ? DefaultValueHandling.Include : DefaultValueHandling.Ignore;
                options.SerializerSettings.NullValueHandling = environment.IsDevelopment() ? NullValueHandling.Include : NullValueHandling.Ignore;
            }).AddMvcOptions(options =>
            {
                //options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
                //options.OutputFormatters.Add(new CsvOutputFormatter(new CsvFormatterOptions()
                //{
                //    CsvDelimiter = ";",
                //    UseSingleLineHeaderInCsv = true
                //}));
            });

            return services;
        }

        /// <summary>
        /// Añade los ProblemDetails por defecto 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IServiceCollection AddSmartProblemDetails(this IServiceCollection services, IHostEnvironment environment)
        {
            return services.AddProblemDetails(options =>
             {
                 options.IncludeExceptionDetails = _ => environment.IsDevelopment();
                 options.Map<NotImplementedException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));
                 options.Map<HttpRequestException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status503ServiceUnavailable));
                 options.Map<Exception>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
             });
        }

        /// <summary>
        /// Añade los ProblemDetails por defecto y permite configurarlo
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        /// <param name="configure"></param>
        public static IServiceCollection AddSmartProblemDetails(this IServiceCollection services, IHostEnvironment environment, Action<ProblemDetailsOptions> configure)
        {
            return services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = _ => environment.IsDevelopment();
                options.Map<NotImplementedException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));
                options.Map<HttpRequestException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status503ServiceUnavailable));
                options.Map<Exception>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
                configure?.Invoke(options);
            });
        }

        /// <summary>
        /// Añade los ProblemDetails por defecto 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IHealthChecksBuilder AddSmartHealthChecks<TContext>(this IServiceCollection services) where TContext : SmartDbContext 
            => services.AddHealthChecks().AddDbContextCheck<TContext>();

#pragma warning disable EF1001 // Internal EF Core API usage.
        /// <summary>
        /// Añade los ProblemDetails por defecto 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        public static IServiceCollection AddSmartDbContextPool<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction) where TContext : SmartDbContext
        {
            services.AddDbContextPool<TContext>(optionsAction);
            services.AddHttpContextAccessor();
            return services.AddScoped(sp =>
            {
                IHttpContextAccessor accessor = sp.GetService<IHttpContextAccessor>();
                TContext context = sp.GetService<DbContextPool<TContext>.Lease>().Context;
                context.CurrentUser = accessor.HttpContext.GetUsername();
                return context;
            });
        }
#pragma warning restore EF1001 // Internal EF Core API usage.

        /// <summary>
        /// Añade serilog con Seq y valores por defecto
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureLogger"></param>
        /// <param name="apiKEy"></param>
        /// <returns></returns>
        public static IHostBuilder UseSmartSerilog(this IHostBuilder builder, Action<HostBuilderContext, LoggerConfiguration> configureLogger = null)
        {
            return builder.ConfigureLogging((ctx, logger) => // Use the default console logger because it's more visual than Serilog one.
            {
                if (!ctx.HostingEnvironment.IsProduction()) logger.AddConsole();
            }).UseSerilog((ctx, options) =>
            {
                //SmartConfiguracion configuracion = ctx.Configuration
                //    .GetSection(nameof(SmartConfiguracion))
                //    .Get< SmartConfiguracion();

                //LogEventLevel logLevel = Enum.Parse<LogEventLevel>(configuracion.Logging.LogLevel);

                LoggingLevelSwitch logSwitch = new LoggingLevelSwitch(/*logLevel*/);
                const string logPath = /*configuracion.Logging.FilePath ?? $@"C:\logs\{configuracion.NombreAplicacion}\logs.json"*/ "log.txt";
                int retainedFileCountLimit = ctx.HostingEnvironment.IsDevelopment() ? 7 : 365 * 2;

                LoggerConfiguration builder = options
                    .ReadFrom.Configuration(ctx.Configuration)
                    .MinimumLevel.ControlledBy(logSwitch)
                    .MinimumLevel.Override("HealthChecks", LogEventLevel.Fatal)
                    .MinimumLevel.Override("System.Net.Http.HttpClient.health-checks", LogEventLevel.Error)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Async(configure => configure.Debug()
                        .WriteTo.File(new CompactJsonFormatter(), logPath, buffered: true, shared: false, flushToDiskInterval: TimeSpan.FromSeconds(5),
                            retainedFileCountLimit: retainedFileCountLimit, rollingInterval: RollingInterval.Day))
                    /*.WriteTo.Seq(configuracion.Logging.SeqUrl, apiKey: configuracion.Logging.SeqApiKey, controlLevelSwitch: logSwitch)*/;

                configureLogger?.Invoke(ctx, builder);
            }, writeToProviders: true);
        }
    }
}
