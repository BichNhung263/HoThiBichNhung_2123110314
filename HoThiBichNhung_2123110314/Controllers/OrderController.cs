using HoThiBichNhung_2123110314.Data;
using HoThiBichNhung_2123110314.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoThiBichNhung_2123110314.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // 🔥 GET ALL (User + OrderDetails)
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .Select(o => new
                {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAt,

                    User = new
                    {
                        o.User.Id,
                        o.User.Name,
                        o.User.Email
                    },

                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.Id,
                        od.ProductId,
                        od.Price,
                        od.Quantity
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

        // 🔥 GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(long id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAt,

                    User = new
                    {
                        o.User.Id,
                        o.User.Name,
                        o.User.Email
                    },

                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.Id,
                        od.ProductId,
                        od.Price,
                        od.Quantity
                    })
                })
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            return Ok(order);
        }

        // 🔥 POST
        [HttpPost]
        public async Task<IActionResult> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // 🔥 PUT (FIX lỗi 500)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(long id, Order order)
        {
            var existing = await _context.Orders.FindAsync(id);

            if (existing == null)
                return NotFound("Không tìm thấy đơn hàng");

            existing.UserId = order.UserId;
            existing.TotalPrice = order.TotalPrice;
            existing.Status = order.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔥 DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}