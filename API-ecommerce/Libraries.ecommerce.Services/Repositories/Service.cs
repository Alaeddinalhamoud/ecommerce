using ecommerce.Data;
using IdentityModel.Client;
using Libraries.ecommerce.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libraries.ecommerce.Services.Repositories
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly Service service;
        public Service(Service _service)
        {
            service = _service;
        }
        public async Task<POJO> Delete(int? id, string url, string accessToken)
        {
            service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await service.Client.DeleteAsync(url + id);
            response.EnsureSuccessStatusCode();
            var Content = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(Content))
            {
                return JsonSerializer.Deserialize<POJO>(Content);
            }
            return null;
        }

        public async Task<T> Get(string id, string url, string accessToken)
        {
            service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await service.Client.GetAsync(url + id);
            response.EnsureSuccessStatusCode();
            var Content = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(Content))
            {
                return JsonSerializer.Deserialize<T>(Content,
                                                  new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            return null;
        }

        public async Task<string> GetAccessToken()
        {
            var disco = await service.Client.GetDiscoveryDocumentAsync(service.websiteSetting.APIAuthUrl);
            // request token
            var tokenResponse = await service.Client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "servertoserver",
                ClientSecret = service.websiteSetting.AuthServerSecret,
                Scope = "ecommerceapi"
            });
            return tokenResponse.AccessToken;
        }

        public async Task<IQueryable<T>> GetAll(string url, string accessToken)
        {
            service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await service.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var Content = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(Content))
            {
                return JsonSerializer.Deserialize<IEnumerable<T>>(Content).AsQueryable();
            }
            return null;
        }

        public async Task<string> GetByPost(T t, string url, string accessToken)
        {
            service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = JsonSerializer.Serialize(t);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await service.Client.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadAsStringAsync());
        }

        public async Task<POJO> Post(T t, string url, string accessToken)
        {
            service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = JsonSerializer.Serialize(t);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await service.Client.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            var Content = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(Content))
            {
                return JsonSerializer.Deserialize<POJO>(Content);
            }
            return null;
        }

        public async Task<POJO> PostRange(List<T> t, string url, string accessToken)
        {
            service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = JsonSerializer.Serialize(t);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await service.Client.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            var Content = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(Content))
            {
                return JsonSerializer.Deserialize<POJO>(Content);
            }
            return null;
        }
    }
}
