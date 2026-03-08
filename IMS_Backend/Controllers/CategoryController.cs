using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController(MyApplicationDB context) : ControllerBase
    {
        public readonly MyApplicationDB _context=context;
        

        [HttpGet]
        [Route("GetCategory")]
        public IActionResult GetCategory()
        {
            int UserId=Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var category = _context.ProdCategory.Where(c => c.UserId == UserId);


            return Ok(category);
            //return View();
        }
    }
}
