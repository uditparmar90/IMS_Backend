using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly MyApplicationDB _context;
        public UsersController(MyApplicationDB context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<ClsUsers>> insertUser(ClsUsers user)
        {
            _context.ClsUsers.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
