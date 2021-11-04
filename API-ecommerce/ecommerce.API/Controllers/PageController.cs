using System.Linq;
using System.Threading.Tasks;
using ecommerce.Data;
using ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PageController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public PageController(IUnitOfWork _UnitOfWork)
        {
            unitOfWork = _UnitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Page page)
        {
            POJO model = new POJO();

            if (page == null)
            {
                model.flag = false;
                model.message = "Empty request.";
                return Ok(model);
            }

            model = await unitOfWork.Page.Save(page);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick.";
                return Ok(model);
            }

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Page model = await unitOfWork.Page.Get(id);

            if (model == null)
            {
                return NotFound();
            }
            if (!model.published)
            {
                return NotFound("Page UnPublised");
            }
            return Ok(model);
        }

        [HttpGet("GetPageId/{id}")]
        public async Task<IActionResult> GetPageById(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Page model = await unitOfWork.Page.Get(id);

            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("PageByNav/{id}")]
        public async Task<IActionResult> PageByNav(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Navs navs = Navs.Main;

            switch (id)
            {
                case "Main":
                    navs = Navs.Main;
                    break;
                case "Information":
                    navs = Navs.Information;
                    break;
                case "Extras":
                    navs = Navs.Extras;
                    break;
                default:
                    navs = Navs.Main;
                    break;
            }

            var models = await unitOfWork.Page.GetAll(x => x.nav.Equals(navs) && x.published.Equals(true));
            if (models == null)
            {
                return NotFound();
            }
            return Ok(models);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IQueryable<Page> model = await unitOfWork.Page.GetAll();
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
            model = await unitOfWork.Page.Delete(id);

            if (model == null)
            {
                model.flag = false;
                model.message = "APIs function is sick";
                return Ok(model);
            }

            return Ok(model);
        }
    }
}
