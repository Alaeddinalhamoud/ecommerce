using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MetaTagController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public MetaTagController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MetaTag metaTag)
        {
            POJO model = new POJO();

            if (metaTag == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.MetaTag.Save(metaTag);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        //Get the Main site tag
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            try
            {
                if (id == 0)
                {
                    //This methods will return only one line.
                    var models = await unitOfWork.MetaTag.GetAll(x => x.metaTagType.Equals(MetaTagType.Site));
                    MetaTag metaTag = models?.FirstOrDefault();
                    return Ok(metaTag);
                }
                else
                {
                    var model = await unitOfWork.MetaTag.GetAll(x => x.productId.Equals(id));
                    return Ok(model?.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.ToString());
            }
        }
        //By productID
        [HttpGet("GetProductMetaTag/{id}")]
        public async Task<IActionResult> GetProductMetaTag(string id)
        {
            IQueryable<MetaTag> model = await unitOfWork.MetaTag.GetAll(x => x.productId.Equals(id));
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model?.FirstOrDefault());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<MetaTag> model = await unitOfWork.MetaTag.GetAll();
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            POJO model = new POJO();
            if (id == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }
            model = await unitOfWork.MetaTag.Delete(id);
            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }
            return Ok(model);
        }

        public async Task<string> GetToken()
        {
            return await HttpContext?.GetTokenAsync("access_token");
        }
    }
}
