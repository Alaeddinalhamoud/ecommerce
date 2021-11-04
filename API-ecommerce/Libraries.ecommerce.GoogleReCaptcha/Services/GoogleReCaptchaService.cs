using Libraries.ecommerce.GoogleReCaptcha.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;

namespace Libraries.ecommerce.GoogleReCaptcha.Services
{
    public class GoogleReCaptchaService : IGoogleReCaptchaService
    {
        private readonly GoogleReCaptchaSettings _googleReCaptchaSettings;
        private readonly IHttpClientFactory _clientFactory;
        string url = "https://www.google.com/recaptcha/api/siteverify";
        public GoogleReCaptchaService(IHttpClientFactory clientFactory, IOptions<GoogleReCaptchaSettings> googleReCaptchaSettings)
        {
            _googleReCaptchaSettings = googleReCaptchaSettings.Value;
            _clientFactory = clientFactory;
        }
        public async Task<bool> GoogleReCaptchaVerification(GoogleReCaptchaRequest googleReCaptchaRequest)
        {
            googleReCaptchaRequest.secret = _googleReCaptchaSettings.ReCaptchaSecretKey;
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetStringAsync($"{url}?secret={googleReCaptchaRequest.secret}&response={googleReCaptchaRequest.response}");
                if (!String.IsNullOrEmpty(response))
                {
                    var jsonResponse = JsonSerializer.Deserialize<GoogleReCaptchaResponse>(response);
                    if (jsonResponse.success || jsonResponse.score > (double)0.5)
                        return true;
                }
            }
            return false;
        }
    }
}
