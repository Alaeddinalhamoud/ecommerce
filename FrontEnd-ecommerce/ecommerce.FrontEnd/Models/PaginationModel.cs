using ecommerce.Data.MVData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ecommerce.FrontEnd.Models
{
    public class PaginationModel
    {
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public List<ProductDetail> Data { get; set; }
    }
}
