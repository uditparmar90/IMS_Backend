using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class UsersController : ControllerBase
    {
        private readonly MyApplicationDB _context;
        public UsersController(MyApplicationDB context)
        {
            _context = context;
        }
        //[HttpPost]
        //public async Task<ActionResult<ClsUsers>> InsertUser(ClsUsers user)
        //{
        //    if (await _context.ClsUsers.AnyAsync(u => u.Email == user.Email))
        //    {
        //        return BadRequest("This email already exists.");
        //    }

        //    _context.ClsUsers.Add(user);
        //    await _context.SaveChangesAsync();
        //    return Created();
        //}
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ClsUsers>> GetUserById([FromBody] int id, string password)
        //{
        //    var user = await _context.ClsUsers.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        user.Password = password;
        //        return Ok(true);
        //    }
        //    ;
        //}
        [HttpPatch("{ID}")]
        public async Task<IActionResult> UpdateUser(int ID, ClsUsers users)
        {
            var existinguser = await _context.ClsUsers.FindAsync(ID);
            if (existinguser == null)
            {
                return NotFound($"User with ID {ID} not found.");

            }
            if (!string.IsNullOrEmpty(users.Name))
                existinguser.Name = users.Name;

            if (!string.IsNullOrEmpty(users.Email))
                existinguser.Email = users.Email;

            if (!string.IsNullOrEmpty(users.Password))
                existinguser.Password = users.Password;

            if (!string.IsNullOrEmpty(users.Role))
                existinguser.Role = users.Role;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Error updating user:" + ex.Message);
            }
        }

    };
}
