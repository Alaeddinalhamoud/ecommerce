using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface ITrackingOrder : IServices<TrackingOrder>
    {
        Task<POJO> Save(TrackingOrder trackingOrder);
    }
}
