
namespace UtilityFunctions.Data
{
    public class FileUploader
    {

    }
    public class FileUploaderRequest
    {
        public string fileUploadAPIKey { get; set; }
        public string contentType { get; set; }
        public string fileExtention { get; set; }
        public string content { get; set; }
        public string sourceController { get; set; }
        public int id { get; set; }
        public string userId { get; set; }
    }

    public class FileUploaderResponse
    {
        public string url { get; set; }
        public double lenght { get; set; }
        public bool flag { get; set; }
        public string message { get; set; }
    }

}
