﻿using ecommerce.Data;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public interface IOrderComplaintCase : IServices<OrderComplaintCase>
    {
        Task<POJO> Save(OrderComplaintCase orderComplaintCase);
    }
}
