using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IPaymentTransaction : IServices<PaymentTransaction>
    {
        Task<POJO> Save(PaymentTransaction paymentTransaction);
    }
}
