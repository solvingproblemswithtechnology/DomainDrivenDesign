namespace SmartDomainDrivenDesign.Infrastructure.MediatR
{
    internal interface ICacheableRequest<TResponse>
    {
        object GetExpirationTime();
        object GetCacheKey();
    }
}