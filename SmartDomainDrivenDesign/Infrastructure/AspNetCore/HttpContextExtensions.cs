using Microsoft.AspNetCore.Http;

namespace SmartDomainDrivenDesign.Infrastructure.AspNetCore
{
    public static class HttpContextExtensions
    {
        public static string GetUsername(this HttpContext context) => context?.User?.Identity?.Name;
    }
}
