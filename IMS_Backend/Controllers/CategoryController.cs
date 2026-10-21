using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
        [Route("GetCategory/{id}")]
        public IActionResult GetCategory(int Id)
        {
            int UserTd = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ProductCategory record= _context.ProdCategory.FirstOrDefault(i=>i.UserId== UserTd && i.Id==Id);
            return Ok(record);
        }


        //performs Insert & Update operations
        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(ProductCategory category)
        {
            int UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int RowsAffected;

            if (category.Id > 0)
            {
                try
                {
                    ProductCategory obj = _context.ProdCategory.FirstOrDefault(item => item.UserId == UserId && item.Id == category.Id);

                    if (obj != null)
                    {
                        obj.Name = category.Name;
                        obj.IsActive = category.IsActive;

                        _context.ProdCategory.Update(obj);
                        RowsAffected= _context.SaveChanges();
                        //return Ok(obj.Id);
                    }
                    else
                    {
                        return NoContent();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                try
                {

                    ProductCategory obj = new()
                    {
                        UserId = UserId,
                        Name = category.Name,
                        IsActive = category.IsActive
                    };

                    Console.WriteLine("You insert : " + obj);
                    _context.ProdCategory.Add(obj);
                    RowsAffected = _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    RowsAffected = 0;
                }

            }

            bool IsInserted = RowsAffected > 0 ? true : false;
            return Ok(IsInserted);


        }
        [HttpDelete]
        [Route("Delete/{Id}")]
        public IActionResult Delete(int Id)
        {
            int UserId=Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (Id > 0)
            {
                ProductCategory obj = _context.ProdCategory.FirstOrDefault(item => item.UserId == UserId && item.Id == Id);
                bool isAssigned = _context.Products.Any(item => item.Category_id ==Id);
                if (isAssigned)
                {
                    return Conflict(new { message = "This category is assigned to products and cannot be deleted." });
                }
                else
                {
                    _context.ProdCategory.Remove(obj);
                    _context.SaveChanges();
                }
                return Ok(Id);
            }
            else
            {
                return NoContent();
            }
        }

        //[HttpPut]
        //[Route("Update")]
        //public IActionResult Update(ProductCategory category)
        //{
        //    int UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    ProductCategory obj = _context.ProdCategory.FirstOrDefault(item => item.UserId == UserId && item.Id == category.Id);

        //    if (obj!=null)
        //    {
        //        obj.Name = category.Name;

        //        _context.ProdCategory.Update(obj);
        //        _context.SaveChanges();
        //        return Ok(obj.Id);
        //    }
        //    else
        //    {
        //        return NoContent();
        //    }
        //}
    }
}
