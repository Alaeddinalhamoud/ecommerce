using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
  public  interface ICartLine : IServices<CartLine>
    {
        Task<POJO> Save(CartLine cartLine); 
    }
}
