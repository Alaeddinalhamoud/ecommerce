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
    public class PaymentService
    {

        private readonly WebsiteSetting websiteSetting;
        public PaymentService(IOptions<WebsiteSetting> _websiteSetting)
        {
            websiteSetting = _websiteSetting.Value;
        }


        public async Task<PaymentResponse> Pay(PaymentRequest paymentRequest)
        { 
            var content = JsonSerializer.Serialize(paymentRequest);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            PaymentResponse paymentResponse = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(websiteSetting.PaymentUrl);
                client.DefaultRequestHeaders.Add("x-functions-key", websiteSetting.PaymentAPIKey);
                var response = await client.PostAsync("/api/Payment", httpContent);
                response.EnsureSuccessStatusCode();
                paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(await response.Content.ReadAsStringAsync());
            }
            return paymentResponse;
        }
    }
}
