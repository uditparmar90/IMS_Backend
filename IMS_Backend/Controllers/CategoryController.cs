using IMS_Backend.DBCommection;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        public readonly MyApplicationDB _context;
        public CategoryController(MyApplicationDB context)
        {
            _context=context;
        }
        [HttpGet]
        public IActionResult GetCate()
        {
             return Ok(_context.ProdCategory.ToList());
            //return View();
        }
    }
}
