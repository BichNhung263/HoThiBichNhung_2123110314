using HoThiBichNhung_2123110314.Data;
using HoThiBichNhung_2123110314.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoThiBichNhung_2123110314.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartItemsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CartItemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> Get()
            => await _context.CartItems.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<CartItem>> Post(CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}