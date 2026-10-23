using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMS_Backend.Controllers
{
    public class ProductTdo
    {
        public int UserId { get; set; }
        public IFormFile? Image { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Category_id { get; set; }
        public int SubCategory_id { get; set; }
        public required string SKU { get; set; }
        public decimal Original_Cost { get; set; }
        public decimal Reorder_level { get; set; }
        public bool IsActive { get; set; }
        public int? Quantity { get; set; }
    };

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController(MyApplicationDB context) : ControllerBase
    {
        readonly MyApplicationDB _context = context;

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromForm] ProductTdo prodDto)
        {
            try
            {
                if (!TryGetUserId(out int userId))
                    return Unauthorized();

                var skuConflict = SkuConflict(userId, prodDto.SKU, excludeProductId: null);
                if (skuConflict != null)
                    return skuConflict;

                byte[]? imageBytes = await ReadImageBytes(prodDto.Image);

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
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var products = _context.Products
                .Where(prod => prod.UserId == userId)
                .OrderByDescending(p => p.Id)
                .ToList();
            return Ok(products);
        }

        [HttpGet]
        [Route("GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var product = FindOwnedProduct(id, userId);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var product = FindOwnedProduct(id, userId);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            bool isUsed = _context.transactions.Any(t => t.prodId == id && t.UserId == userId);
            if (isUsed)
                return Conflict(new { message = "This product is used in sales and cannot be deleted." });

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok(id);
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductTdo prodDto)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var existingProduct = FindOwnedProduct(id, userId);
            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            var skuConflict = SkuConflict(userId, prodDto.SKU, excludeProductId: id);
            if (skuConflict != null)
                return skuConflict;

            if (prodDto.Image != null && prodDto.Image.Length > 0)
            {
                existingProduct.Image = await ReadImageBytes(prodDto.Image);
            }

            existingProduct.Name = prodDto.Name;
            existingProduct.Price = prodDto.Price;
            existingProduct.Category_id = prodDto.Category_id;
            existingProduct.SubCategory_id = prodDto.SubCategory_id;
            existingProduct.SKU = prodDto.SKU;
            existingProduct.Original_Cost = prodDto.Original_Cost;
            existingProduct.Reorder_level = prodDto.Reorder_level;
            existingProduct.Quantity = prodDto.Quantity;
            existingProduct.IsActive = prodDto.IsActive;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"{prodDto.Name} updated successfully", productId = id });
        }

        bool TryGetUserId(out int userId)
        {
            var strUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(strUserId))
            {
                userId = 0;
                return false;
            }

            userId = Convert.ToInt32(strUserId);
            return true;
        }

        Products? FindOwnedProduct(int id, int userId)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id && p.UserId == userId);
        }

        IActionResult? SkuConflict(int userId, string sku, int? excludeProductId)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return BadRequest(new { message = "SKU is required." });

            var query = _context.Products.Where(p =>
                p.UserId == userId &&
                p.SKU != null &&
                p.SKU.ToLower() == sku.Trim().ToLower());

            if (excludeProductId.HasValue)
                query = query.Where(p => p.Id != excludeProductId.Value);

            if (query.Any())
                return Conflict(new { message = "A product with this SKU already exists." });

            return null;
        }

        static async Task<byte[]?> ReadImageBytes(IFormFile? image)
        {
            if (image == null || image.Length == 0)
                return null;

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
