using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDomainDrivenDesign.Infrastructure.AspNetCore
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class BaseController<TContext> : ControllerBase where TContext : DbContext
    {
        protected readonly TContext context;
        protected readonly ILogger<BaseController<TContext>> logger;
        protected readonly IMapper mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        protected BaseController(TContext context, ILogger<BaseController<TContext>> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
    }
}
