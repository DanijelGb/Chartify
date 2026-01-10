
namespace Chartify.Application.Interfaces;

public interface IChacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan ttl);
}