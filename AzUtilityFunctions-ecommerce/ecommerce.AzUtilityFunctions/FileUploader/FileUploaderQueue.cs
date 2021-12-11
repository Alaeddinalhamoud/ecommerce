using ecommerce.Data;
using Libraries.ecommerce.Services.Services;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.FileUploader
{
    public class FileUploaderQueue
    {
        private readonly IService<Category> serviceCategory;
        private readonly string urlCategory = "/api/category/";
        private readonly IService<Brand> serviceBrand;
        private readonly string urlBrand = "/api/brand/";
        //Calling Media service.
        private readonly IService<Media> serviceMedia;
        private readonly string urlMedia = "/api/media/";
        private readonly IService<Slider> serviceSlider;
        private readonly string urlSlider = "/api/slider/";
        private readonly IService<Setting> serviceSetting;
        private readonly string urlSetting = "/api/setting/";
        private readonly IService<VendorMedia> serviceVendorMedia;
        private readonly string urlVendorMedia = "/api/VendorMedia/";
        private readonly IService<OrderReturnCase> serviceOrderReturnCase;
        private readonly string urlOrderReturnCase = "/api/OrderReturnCase/";

        public FileUploaderQueue(IService<Category> _serviceCategory, IService<Brand> _serviceBrand,
            IService<Media> _serviceMedia, IService<Slider> _serviceSlider, IService<Setting> _serviceSetting,
            IService<VendorMedia> _serviceVendorMedia, IService<OrderReturnCase> _serviceOrderReturnCase)
        {
            serviceCategory = _serviceCategory;
            serviceBrand = _serviceBrand;
            serviceMedia = _serviceMedia;
            serviceSlider = _serviceSlider;
            serviceSetting = _serviceSetting;
            serviceVendorMedia = _serviceVendorMedia;
            serviceOrderReturnCase = _serviceOrderReturnCase;
        }

        [FunctionName("FileUploaderQueue")]
        public async Task Run(
            [QueueTrigger("files-submit", Connection = "AzureWebJobsStorage")] string myFilesItem,
            [Blob("files", FileAccess.ReadWrite)] CloudBlobContainer myFilesBlob,
            [Blob("files-submit", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer filesSubmitContainer,
            [Blob("files-archive", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer filesArchiveContainer,
            ILogger log)
        {
            FileUploaderResponse fileUploaderResponse = new FileUploaderResponse();
            log.LogInformation($"Queue trigger function processed: {myFilesItem}");

            try
            {
                var token = await serviceBrand.GetAccessToken();
                var filesSubmit = filesSubmitContainer.GetBlockBlobReference(myFilesItem);
                await filesArchiveContainer.CreateIfNotExistsAsync();
                await myFilesBlob.CreateIfNotExistsAsync();
                var filesArchive = filesArchiveContainer.GetBlockBlobReference(myFilesItem);

                var fileUploaderRequest = JsonSerializer.Deserialize<FileUploaderRequest>(await filesSubmit.DownloadTextAsync());

                string randomFileName = "";
                byte[] fileData = Convert.FromBase64String(fileUploaderRequest.content);
                string folder = null;
                if (fileUploaderRequest.contentType.StartsWith("image/"))
                {
                    folder = "images/";
                    randomFileName = $"Image_{Guid.NewGuid().ToString()}{fileUploaderRequest.fileExtention}";
                }
                else if (fileUploaderRequest.contentType.StartsWith("video/"))
                {
                    folder = "videos/";
                    randomFileName = $"Video_{Guid.NewGuid().ToString()}{fileUploaderRequest.fileExtention}";
                }
                else if (fileUploaderRequest.contentType.Equals("application/pdf"))
                {
                    folder = "files/";
                    randomFileName = $"File_{Guid.NewGuid().ToString()}{fileUploaderRequest.fileExtention}";
                }

                CloudBlockBlob blockBlob = myFilesBlob.GetBlockBlobReference(folder + randomFileName);
                await blockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);

                fileUploaderResponse.flag = true;
                fileUploaderResponse.lenght = blockBlob.Properties.Length;
                fileUploaderResponse.url = blockBlob.Uri.ToString();
                fileUploaderResponse.message = "File has been uploaded.";
                //Get the image url from the blob to be send to db
                fileUploaderRequest.content = blockBlob.Uri.ToString();
                log.LogInformation("File has been uploaded to the storage.");

                //Update DataBase Big Shittttt 
                POJO model = new POJO();
                if (fileUploaderRequest.sourceController.Equals("Category"))
                {
                    model = await UpdateImageCategoty(fileUploaderRequest, token);
                }
                else if (fileUploaderRequest.sourceController.Equals("Brand"))
                {
                    model = await UpdateImageBrand(fileUploaderRequest, token);
                }
                else if (fileUploaderRequest.sourceController.Equals("Slider"))
                {
                    model = await UpdateImageSlider(fileUploaderRequest, token);
                }
                else if (fileUploaderRequest.sourceController.Equals("Setting"))
                {
                    model = await UpdateMediaSettings(fileUploaderRequest, token);
                }
                else if (fileUploaderRequest.sourceController.Equals("Product"))
                {
                    model = await UpdateImageProduct(fileUploaderRequest, token);
                }
                else if (fileUploaderRequest.sourceController.Equals("VendorApplication"))
                {
                    model = await AddVendorApplicationMedia(fileUploaderRequest, token);
                }
                else if (fileUploaderRequest.sourceController.Equals("OrderReturnCase"))
                {
                    model = await UpdateMediaOrderReturnCase(fileUploaderRequest, token);
                }

                if (model.flag.Equals(true))
                {
                    log.LogInformation("File has been sent to Files Archive.");
                    await filesArchive.StartCopyAsync(filesSubmit);
                    log.LogInformation("File has been Deleted From Submit Blob.");
                    await filesSubmit.DeleteAsync();
                }
                else
                {
                    log.LogError($"error msg : In the File Uploader {model.message.ToString()}");
                }

            }
            catch (Exception ex)
            {
                log.LogError($"error msg : {ex.ToString()}");
                fileUploaderResponse.flag = false;
                fileUploaderResponse.message = ex.ToString();
            }

        }

        //Categry Update
        public async Task<POJO> UpdateImageCategoty(FileUploaderRequest fileUploaderRequest, string token)
        {
            Category category = await serviceCategory.Get(fileUploaderRequest.id.ToString(), urlCategory, token);
            category.imagePath = fileUploaderRequest.content;
            category.updateDate = DateTime.Now;
            category.createdBy = fileUploaderRequest.userId;
            return await serviceCategory.Post(category, urlCategory, token);
        }
        //Brand Update
        public async Task<POJO> UpdateImageBrand(FileUploaderRequest fileUploaderRequest, string token)
        {
            Brand brand = await serviceBrand.Get(fileUploaderRequest.id.ToString(), urlBrand, token);
            brand.imagePath = fileUploaderRequest.content;
            brand.updateDate = DateTime.Now;
            brand.createdBy = fileUploaderRequest.userId;
            return await serviceBrand.Post(brand, urlBrand, token);
        }
        //Slider Update
        public async Task<POJO> UpdateImageSlider(FileUploaderRequest fileUploaderRequest, string token)
        {
            Slider slider = await serviceSlider.Get(fileUploaderRequest.id.ToString(), urlSlider, token);

            slider.imagePath = fileUploaderRequest.content;
            slider.updateDate = DateTime.Now;
            slider.createdBy = fileUploaderRequest.userId;
            return await serviceSlider.Post(slider, urlSlider, token);
        }

        //Settings (Logo) Update
        public async Task<POJO> UpdateMediaSettings(FileUploaderRequest fileUploaderRequest, string token)
        {
            var settings = await serviceSetting.GetAll(urlSetting, token);
            var setting = settings?.FirstOrDefault();

            if (fileUploaderRequest.contentType.Equals("application/pdf"))
            {
                setting.vendorAgreementContract = fileUploaderRequest.content;
            }
            else
            {
                setting.logo = fileUploaderRequest.content;
            }

            setting.updateDate = DateTime.Now;
            setting.createdBy = fileUploaderRequest.userId;
            return await serviceSetting.Post(setting, urlSetting, token);
        }

        //Update Media OrderReturn Case Image
        public async Task<POJO> UpdateMediaOrderReturnCase(FileUploaderRequest fileUploaderRequest, string token)
        {
            OrderReturnCase orderReturnCase = await serviceOrderReturnCase.Get(fileUploaderRequest.id.ToString(), urlOrderReturnCase, token);


            orderReturnCase.imageUrl = fileUploaderRequest.content;
            orderReturnCase.updateDate = DateTime.Now;
            orderReturnCase.createdBy = fileUploaderRequest.userId;

            return await serviceOrderReturnCase.Post(orderReturnCase, urlOrderReturnCase, token);
        }

        //Update Product
        public async Task<POJO> UpdateImageProduct(FileUploaderRequest fileUploaderRequest, string token)
        {
            Media media = new Media()
            {
                productId = fileUploaderRequest.id,
                path = fileUploaderRequest.content,
                createDate = DateTime.Now,
                updateDate = DateTime.Now,
                createdBy = fileUploaderRequest.userId,
                mediaType = fileUploaderRequest.fileExtention == ".pdf" ? MediaType.File : MediaType.Image,
                modifiedBy = fileUploaderRequest.userId,
                alt = fileUploaderRequest.fileExtention
            };
            return await serviceMedia.Post(media, urlMedia, token);
        }
        //Update Vendor
        public async Task<POJO> AddVendorApplicationMedia(FileUploaderRequest fileUploaderRequest, string token)
        {
            VendorMedia vendorMedia = new VendorMedia()
            {
                vendorId = fileUploaderRequest.userId,
                path = fileUploaderRequest.content,
                createDate = DateTime.Now,
                updateDate = DateTime.Now,
                createdBy = fileUploaderRequest.userId,
                mediaType = fileUploaderRequest.fileExtention == ".pdf" ? MediaType.File : MediaType.Image,
                modifiedBy = fileUploaderRequest.userId,
                alt = fileUploaderRequest.fileExtention
            };
            return await serviceVendorMedia.Post(vendorMedia, urlVendorMedia, token);
        }

    }
}
