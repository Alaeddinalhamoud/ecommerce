using Libraries.ecommerce.Services.Models;
using Microsoft.Extensions.Options;
using System; 
using System.Net.Http; 

namespace Libraries.ecommerce.Services.Services
{
    public class Service
    {
        public HttpClient Client { get; }
        public readonly WebsiteSetting websiteSetting;

        public Service(HttpClient client, IOptions<WebsiteSetting> _websiteSetting)
        {
            websiteSetting = _websiteSetting.Value;
            client.BaseAddress = new Uri(websiteSetting.APIUrl);             
            Client = client;
        } 
    }
}
