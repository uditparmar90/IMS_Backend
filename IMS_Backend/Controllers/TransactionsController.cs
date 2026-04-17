using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;

namespace IMS_Backend.Controllers
{
    public class CartProdTDO
    {
        public int id { get; set; }
        public required string name { get; set; }
        public int prodId { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }


    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController(MyApplicationDB context) : ControllerBase
    {
        readonly MyApplicationDB _context = context;


        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] List< CartProdTDO> check)
        {
            int userid =Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (check.Count > 0)
            {
                var finaldata = new List<Transactions>();
                foreach (var a in check)
                {
                    finaldata.Add(new Transactions
                    {
                        name = a.name,
                        UserId = userid,
                        prodId = a.id,
                        price = a.price,
                        quantity = a.quantity,
                        Original_Cost = 0,
                        tran_Date = DateTime.UtcNow
                    });
                }
                foreach(var prod in finaldata)
                {
                    try
                    {
                        _context.transactions.Add(prod);
                        var products = _context.Products.FirstOrDefault(prod2 =>

                            prod2.UserId == userid && prod.prodId == prod2.Id
                        );
                        if(products != null)
                        {
                            products.Quantity = (int?)(products.Quantity - prod.quantity);
                        }
                        
                        _context.SaveChanges();

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("EX : "+ ex.InnerException?.Message);
                    }
                    
                }
            }
            return Ok();
        }
    }


    
}

   
