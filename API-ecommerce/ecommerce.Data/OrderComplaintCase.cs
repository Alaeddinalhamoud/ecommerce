using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Data
{
    public class OrderComplaintCase
    {
        public int id { get; set; }
        public string orderId { get; set; }
        public int reasonId { get; set; }
        public string customerNote { get; set; }
        public Status status { get; set; }
        public string messageToCustomer { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime createDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime updateDate { get; set; }
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        public string actionsToSolve { get; set; }
    }
}
