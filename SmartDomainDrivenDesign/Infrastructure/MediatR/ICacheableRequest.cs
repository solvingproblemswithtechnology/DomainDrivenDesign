namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    public interface ICacheableRequest<TResponse>
    {
        object GetExpirationTime();
        object GetCacheKey();
    }
}