using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ISlider : IServices<Slider>
    {
        Task<POJO> Save(Slider slider);
    }
}
