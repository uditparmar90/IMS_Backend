using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace IMS_Backend.Controllers
{
    public class ProductTdo     
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public IFormFile? Image { get; set; }          // <-- changed to IFormFile?
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Category_id { get; set; }
        public int SubCategory_id { get; set; }
        //..Stock kipping Unit
        public required string SKU { get; set; }
        public Decimal Original_Cost { get; set; }
        public Decimal Reorder_level { get; set; }
        public bool IsActive { get; set; }
        public int? Quantity { get; set; }
    };

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController(MyApplicationDB context) : ControllerBase
    {
        readonly MyApplicationDB _context= context;
        readonly int UserId;


        //[HttpPost("Insert")] 
        //public IActionResult Insert([FromForm] ProductTdo prodDto)
        //{
        //    try
        //    {
        //        var StrUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        //        Console.WriteLine("userid : " + StrUserId);
        //        int UserId = Convert.ToInt32(StrUserId);

        //        var newProduct = new Products
        //        {
        //            UserId = UserId,
        //            Name = prodDto.Name,
        //            Price = prodDto.Price,
        //            Category_id = prodDto.Category_id,
        //            SubCategory_id = prodDto.SubCategory_id,
        //            SKU = prodDto.SKU,
        //            Original_Cost = prodDto.Original_Cost,
        //            Reorder_level = prodDto.Reorder_level,
        //            Quantity=prodDto.Quantity,
        //            IsActive = prodDto.IsActive
        //        };

        //        var msg = _context.Products.AddAsync(newProduct);
        //        Debug.WriteLine(msg);
        //        _context.SaveChanges();
        //        return Ok(new { message = "Product added successfully", productId = newProduct.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromForm] ProductTdo prodDto)
        {
            try
            {
                var strUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(strUserId))
                    return Unauthorized();

                int userId = Convert.ToInt32(strUserId);

                byte[]? imageBytes = null;

                if (prodDto.Image != null && prodDto.Image.Length > 0)
                {
                    using var memoryStream = new MemoryStream();

                    await prodDto.Image.CopyToAsync(memoryStream);

                    imageBytes = memoryStream.ToArray();
                }

                var newProduct = new Products
                {
                    UserId = userId,
                    Name = prodDto.Name,
                    Price = prodDto.Price,
                    Category_id = prodDto.Category_id,
                    SubCategory_id = prodDto.SubCategory_id,
                    SKU = prodDto.SKU,
                    Original_Cost = prodDto.Original_Cost,
                    Reorder_level = prodDto.Reorder_level,
                    Quantity = prodDto.Quantity,
                    IsActive = prodDto.IsActive,

                    // Store image in VARBINARY(MAX)
                    Image = imageBytes
                };

                _context.Products.Add(newProduct);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Product added successfully",
                    productId = newProduct.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("GetAllProducts")]
        public List<Products> GetAllProducts()
        {
            var StrUserId=User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            Console.WriteLine("userid : " + StrUserId);
            int UserId = Convert.ToInt32(StrUserId);
            var products = _context.Products.Where(prod =>prod.UserId== UserId).OrderByDescending(p => p.Id).ToList();
            return products;
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public int Delete(int id)
        {
            _context.Products.Remove(_context.Products.Find(id)!);
            _context.SaveChanges();
            return id;  
        }
        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult UpdateProduct(int id, [FromForm] ProductTdo prodDto)
        {
            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            byte[]? imageBytes = null;

            if (prodDto.Image != null && prodDto.Image.Length > 0)
            {
                using var memoryStream = new MemoryStream();

                prodDto.Image.CopyToAsync(memoryStream);

                imageBytes = memoryStream.ToArray();
            }


            existingProduct.Image = imageBytes;
            existingProduct.Name = prodDto.Name;
            existingProduct.Price = prodDto.Price;
            existingProduct.Category_id = prodDto.Category_id;
            existingProduct.SubCategory_id = prodDto.SubCategory_id;
            existingProduct.SKU = prodDto.SKU;
            existingProduct.Original_Cost = prodDto.Original_Cost;
            existingProduct.Reorder_level = prodDto.Reorder_level;
            existingProduct.Quantity = prodDto.Quantity;


            var ProdFromDb = new Products
            {
                //Id = prodDto.Id,
                Name = prodDto.Name,
                Price = prodDto.Price,
                Category_id = prodDto.Category_id,
                SubCategory_id = prodDto.SubCategory_id,
                SKU = prodDto.SKU,
                Original_Cost = prodDto.Original_Cost,
                Reorder_level = prodDto.Reorder_level
            };
            _context.SaveChanges();
            return Ok(new { message = $"{prodDto.Name} updated successfully", productId = id });
        }
    }
}
