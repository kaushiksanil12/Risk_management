using ERMS.Web.Helpers;
using Newtonsoft.Json;
using System.Text;

namespace ERMS.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IHttpContextAccessor _ctx;

        public ApiService(IHttpClientFactory httpFactory, IHttpContextAccessor ctx)
        {
            _httpFactory = httpFactory;
            _ctx = ctx;
        }

        private HttpClient CreateClient()
        {
            var client = _httpFactory.CreateClient("ERMSAPI");
            var session = _ctx.HttpContext?.Session;
            if (session != null)
            {
                var userId = SessionHelper.GetUserId(session);
                var adminFlag = session.GetString(SessionHelper.AdminFlag) ?? "N";
                client.DefaultRequestHeaders.Remove("X-User-Id");
                client.DefaultRequestHeaders.Remove("X-Admin-Flag");
                client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
                client.DefaultRequestHeaders.Add("X-Admin-Flag", adminFlag);
            }
            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _ctx.HttpContext?.Session.Clear();
                return default;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object body)
        {
            var client = CreateClient();
            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _ctx.HttpContext?.Session.Clear();
                return default;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object body)
        {
            var client = CreateClient();
            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _ctx.HttpContext?.Session.Clear();
                return default;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
