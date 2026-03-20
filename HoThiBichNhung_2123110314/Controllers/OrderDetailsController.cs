using HoThiBichNhung_2123110314.Data;
using HoThiBichNhung_2123110314.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoThiBichNhung_2123110314.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostOrderDetail), new { id = detail.Id }, detail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(long id)
        {
            var detail = await _context.OrderDetails.FindAsync(id);
            if (detail == null) return NotFound();

            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}