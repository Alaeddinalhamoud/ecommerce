using Libraries.ecommerce.Services.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace Libraries.ecommerce.Services.Repositories
{
    public class FileUploadService
    {

        private readonly WebsiteSetting websiteSetting;
        public FileUploadService(IOptions<WebsiteSetting> _websiteSetting)
        {
            websiteSetting = _websiteSetting.Value;
        }


        public async Task<FileUploaderResponse> FileUpload(FileUploaderRequest fileUploaderRequest)
        { 
            var content = JsonSerializer.Serialize(fileUploaderRequest);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            FileUploaderResponse fileUploaderResponse = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(websiteSetting.FileUploadUrl);
                client.DefaultRequestHeaders.Add("x-functions-key", websiteSetting.FileUploadAPIKey);
                var response = await client.PostAsync("/api/FileUploader", httpContent);
                response.EnsureSuccessStatusCode();
                fileUploaderResponse = JsonSerializer.Deserialize<FileUploaderResponse>(await response.Content.ReadAsStringAsync());
            }
            return fileUploaderResponse;
        }
    }
}
