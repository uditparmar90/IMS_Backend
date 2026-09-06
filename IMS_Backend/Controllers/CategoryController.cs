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
        [Route("GetAllCategories")]
        public IActionResult GetCategories()
        {
            int UserId=Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var category = _context.ProdCategory.Where(c => c.UserId == UserId);


            return Ok(category);
        }
        [HttpGet]
        [Route("GetCategory")]
        public IActionResult GetCategory(int Id)
        {
            int UserTd = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ProductCategory record= _context.ProdCategory.FirstOrDefault(i=>i.UserId== UserTd && i.Id==Id);
            return Ok(record);
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(ProductCategory category)
        {
            Console.WriteLine("You insert : "+category);
             _context.ProdCategory.Add(category);
            int RowsAffected=_context.SaveChanges();
            bool IsInserted=RowsAffected > 0 ? true : false;
            
            return Ok(IsInserted); 
        }
        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int Id)
        {
            int UserId=Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (Id > 0)
            {
                ProductCategory obj = _context.ProdCategory.FirstOrDefault(item => item.UserId == UserId && item.Id == Id);

                _context.ProdCategory.Remove(obj);
                _context.SaveChanges();
                return Ok(Id);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Update(ProductCategory category)
        {
            int UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ProductCategory obj = _context.ProdCategory.FirstOrDefault(item => item.UserId == UserId && item.Id == category.Id);

            if (obj!=null)
            {
                obj.Name = category.Name;

                _context.ProdCategory.Update(obj);
                _context.SaveChanges();
                return Ok(obj.Id);
            }
            else
            {
                return NoContent();
            }
        }
    }
}
