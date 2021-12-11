namespace UtilityFunctions.Data
{
    public class EmailSender
    {
    }

    public class EmailSenderRequest
    {
        public string senderName { get; set; }
        public string from { get; set; }
        public string subject { get; set; }
        public string to { get; set; }
        public string cc { get; set; }
        public string receiverName { get; set; }
        public string plainTextContent { get; set; }
        public string htmlContent { get; set; }
    }

    public class EmailSenderResponse
    {
        public bool flag { get; set; }
        public string message { get; set; }
    }
}
