namespace ecommerce.Data.Models
{
    public static class StaticValues
    {
        public static string NullDateTime { get; } = "0001-01-01T00:00:00";
        public static string RecordAdded { get; } = "Has Been Added.";
        public static string RecordUpdated { get; } = "Has been Updated.";
        public static string RecordDeleted { get; } = "Has been Deleted.";
        public static string RecordNotFound { get; } = "Does not exist!";
        public static string EmptyAPIRequest { get; } = "You have sent empty request.";
        public static string ErrorResponseAPIDB { get; } = "You have an issue with the API and DB connection.";
        public static string ExceptionResponseAPI { get; } = "APIs function have an issue. Error mssage: ";
    }
}
