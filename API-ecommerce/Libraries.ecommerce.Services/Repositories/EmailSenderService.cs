using Libraries.ecommerce.Services.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace Libraries.ecommerce.Services.Repositories
{
    public class EmailSenderService
    {

        private readonly WebsiteSetting websiteSetting;
        public EmailSenderService(IOptions<WebsiteSetting> _websiteSetting)
        {
            websiteSetting = _websiteSetting.Value;
        }


        public async Task<EmailSenderResponse> SendEmail(EmailSenderRequest emailSenderRequest)
        { 
            var content = JsonSerializer.Serialize(emailSenderRequest);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            EmailSenderResponse emailSenderResponse = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(websiteSetting.EmailSenderUrl);
                client.DefaultRequestHeaders.Add("x-functions-key", websiteSetting.EmailSenderAPIKey);
                var response = await client.PostAsync("/api/EmailSender", httpContent);
                response.EnsureSuccessStatusCode();
                emailSenderResponse = JsonSerializer.Deserialize<EmailSenderResponse>(await response.Content.ReadAsStringAsync());
            }
            return emailSenderResponse;
        }
    }
}
