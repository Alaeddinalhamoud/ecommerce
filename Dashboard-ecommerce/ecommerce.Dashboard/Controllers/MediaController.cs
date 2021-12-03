using System; 
using System.IO; 
using System.Threading.Tasks;
using ecommerce.Dashboard.Models; 
using ecommerce.Data;
using Libraries.ecommerce.Services.Repositories;
using Libraries.ecommerce.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UtilityFunctions.Data;

namespace ecommerce.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MediaController : Controller
    {
        private readonly FileUploadService fileUploadService;
        //Calling Media service.
        private readonly IService<Media> service;
        private readonly string url = "/api/media/";
        private readonly ILogger<MediaController> _logger;
        public MediaController(FileUploadService _FileUploadService, IService<Media> _service, ILogger<MediaController> logger)
        {
            fileUploadService = _FileUploadService;
            service = _service;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upload(FileUploaderRequest fileUploaderRequest)
        {
            if ((fileUploaderRequest.id.Equals(0)) || String.IsNullOrEmpty(fileUploaderRequest.sourceController))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(fileUploaderRequest);
        }         

        //Upoad mutli files
        public IActionResult MultiUpload(FileUploaderRequest fileUploaderRequest)
        {
            if ((fileUploaderRequest.id.Equals(0)) || String.IsNullOrEmpty(fileUploaderRequest.sourceController))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
            return View(fileUploaderRequest);
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormCollection form, FileUploaderRequest fileUploaderRequest)
        {
            try
            {
                fileUploaderRequest.contentType = form.Files[0].ContentType;
                fileUploaderRequest.fileExtention = Path.GetExtension(form.Files[0].FileName);
                fileUploaderRequest.userId = GetCurrentUserId();

                using (var memoryStream = new MemoryStream())
                {
                    await form.Files[0].CopyToAsync(memoryStream);
                    fileUploaderRequest.content = Convert.ToBase64String(memoryStream.ToArray());
                }

                await fileUploadService.FileUpload(fileUploaderRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        } 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            Media media = new Media()
            {
                id = id,
                isDeleted = true,
                updateDate = DateTime.Now,
                modifiedBy = GetCurrentUserId()
            };

            try
            { 
                POJO model = await service.Post(media, $"{url}UpdateIsDeleted", await GetToken());
                return RedirectToAction("Details", "Product", new { id = model.id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                _logger.LogError($"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }

            try
            { 
                Media media = await service.Get(id, url, await GetToken());
                MvMedia mVMedia  = new MvMedia()
                {
                    id = media.id,
                    alt = media.alt,
                    path = media.path,
                    productId = media.productId,
                    mediaType = media.mediaType,
                    name = media.name
                };
                return View(mVMedia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                return RedirectToAction("Error", "Home");
            }
        }

        //Used to update alt text.

        [HttpPost]
        public async Task<IActionResult> Save(MvMedia mVMedia)
        {
            POJO model = new POJO();
            if (ModelState.IsValid)
            {
                try
                { 
                    Media media = new Media()
                    {
                        id = mVMedia.id,
                        alt = mVMedia.alt,
                        modifiedBy = GetCurrentUserId(),
                        createDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        createdBy = mVMedia.id.Equals(0) ? GetCurrentUserId() : null,
                        name = mVMedia.name
                    };
                    model = await service.Post(media, url, await GetToken());
                   
                    if (model.flag)
                    {
                        return RedirectToAction("Details", "Product", new { id = mVMedia.productId });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"UserId: {GetCurrentUserId()}");
                    return RedirectToAction("Error", "Home");
                }
                ModelState.AddModelError("", model.message);
            }
            return View(mVMedia);
        }

        public string GetCurrentUserId()
        {
            return User?.FindFirst(c => c.Type == "sub")?.Value;
        }
        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}