namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    internal interface ICacheInvalidationRequest
    {
        string GetCacheKey();
    }
}