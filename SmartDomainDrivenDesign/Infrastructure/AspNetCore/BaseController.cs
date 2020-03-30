using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SmartDomainDrivenDesign.Infrastructure.AspNetCore
{
    [ApiController]
    public abstract class BaseController<TContext> : ControllerBase where TContext : DbContext
    {
        protected readonly TContext context;
        protected readonly ILogger<BaseController<TContext>> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        protected BaseController(TContext context, ILogger<BaseController<TContext>> logger)
        {
            this.context = context;
            this.logger = logger;
        }
    }
}
