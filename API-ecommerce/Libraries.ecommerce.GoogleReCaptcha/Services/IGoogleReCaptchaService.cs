using Libraries.ecommerce.GoogleReCaptcha.Models;
using System.Threading.Tasks;

namespace Libraries.ecommerce.GoogleReCaptcha.Services
{
    public interface IGoogleReCaptchaService
    {
        Task<bool> GoogleReCaptchaVerification(GoogleReCaptchaRequest googleReCaptchaRequest);
    }
}
