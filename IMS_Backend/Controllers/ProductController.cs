using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace IMS_Backend.Controllers
{
    public class ProductTdo
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Category_id { get; set; }
        public int SubCategory_id { get; set; }
        //..Stock kipping Unit
        public required string SKU { get; set; }
        public Decimal Original_Cost { get; set; }
        public Decimal Reorder_level { get; set; }
        public bool IsActive { get; set; }

    };
    [ApiController]
    [Route("api/[controller]")]

    //[Authorize]
    public class ProductController : ControllerBase
    {
        readonly MyApplicationDB _context;
        readonly IConfiguration _config;
        readonly int UserId;

        public ProductController(IConfiguration config, MyApplicationDB context)
        {
            _context = context;
            //UserId = HttpContext.Session.GetInt32("UserId")??0;
            
        }

        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] ProductTdo prodDto)
        {
            try
            {
                var userid = User.FindFirst(ClaimTypes.Sid);
                int intUserId = Convert.ToInt32(userid);

                var newProduct = new Products
                {
                    //Id = prodDto.Id,
                    Name = prodDto.Name,
                    Price = prodDto.Price,
                    Category_id = prodDto.Category_id,
                    SubCategory_id = prodDto.SubCategory_id,
                    SKU = prodDto.SKU,
                    Original_Cost = prodDto.Original_Cost,
                    Reorder_lavel = prodDto.Reorder_level,
                    IsActive = prodDto.IsActive
                };

                var msg = _context.Products.AddAsync(newProduct);
                Debug.WriteLine(msg);
                _context.SaveChanges();
                return Ok(new { message = "Product added successfully", productId = newProduct.Id });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public List<Products> GetAllProducts()
        {
            int UserId=HttpContext.Session.GetInt32("UserId")??0;
            Console.WriteLine("consoleWL: " +UserId);
                var products = _context.Products.Where(prod =>prod.UserId==1).OrderByDescending(p => p.Id).ToList();
            return products;
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public int delete(int id)
        {
            _context.Products.Remove(_context.Products.Find(id)!);
            _context.SaveChanges();
            return id;  
        }
        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductTdo prodDto)
        {
            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            existingProduct.Name = prodDto.Name;
            existingProduct.Price = prodDto.Price;
            existingProduct.Category_id = prodDto.Category_id;
            existingProduct.SubCategory_id = prodDto.SubCategory_id;
            existingProduct.SKU = prodDto.SKU;
            existingProduct.Original_Cost = prodDto.Original_Cost;
            existingProduct.Reorder_lavel = prodDto.Reorder_level;


            var ProdFromDb = new Products
            {
                //Id = prodDto.Id,
                Name = prodDto.Name,
                Price = prodDto.Price,
                Category_id = prodDto.Category_id,
                SubCategory_id = prodDto.SubCategory_id,
                SKU = prodDto.SKU,
                Original_Cost = prodDto.Original_Cost,
                Reorder_lavel = prodDto.Reorder_level
            };
            _context.SaveChanges();
            return Ok(new { message = "Product added successfully", productId = id });
        }
    }
}
