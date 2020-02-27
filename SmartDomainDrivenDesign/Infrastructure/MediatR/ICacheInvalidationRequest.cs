namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    public interface ICacheInvalidationRequest
    {
        string GetCacheKey();
    }
}