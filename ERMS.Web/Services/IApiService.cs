namespace ERMS.Web.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<T?> PostAsync<T>(string endpoint, object body);
        Task<T?> PutAsync<T>(string endpoint, object body);
    }
}
