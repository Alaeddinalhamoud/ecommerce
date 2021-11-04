
using System.Collections.Generic;

namespace ecommerce.Data.MVData
{
    class MVReport
    {
    }
    public class MVReportNumberOfOrdersMonthly
    {
        public string month { get; set; }
        public string count { get; set; }  
    }
    public class MVReportNumberOfProductReview
    {
        public int productsCount { get; set; }
        public int pendingProductsCount { get; set; }
        public double earningsAnnual { get; set; }
        public int pendingReviewsCount { get; set; }
        public int pendingShippment { get; set; }
        public int pendingReturnOrder { get; set; }
        public int pendingCompliment { get; set; }
    }
    public class MVReportNumberOfPaymentMethod
    {
        public int cash { get; set; }
        public int card { get; set; }
    }

    public class MVReportUserCases
    {
        public IEnumerable<OrderReturnCase> OrderReturnCases { get; set; }
        public IEnumerable<OrderComplaintCase> OrderComplaintCases { get; set; }
    }
}
