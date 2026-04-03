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

        // 🔥 GET ALL (Product + User)
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails()
        {
            var data = await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                    .ThenInclude(o => o.User)
                .Select(od => new
                {
                    od.Id,
                    od.Price,
                    od.Quantity,

                    Product = new
                    {
                        od.Product.Id,
                        od.Product.Name,
                        od.Product.Price
                    },

                    Order = new
                    {
                        od.Order.Id,

                        User = new
                        {
                            od.Order.User.Id,
                            od.Order.User.Name,
                            od.Order.User.Email
                        }
                    }
                })
                .ToListAsync();

            return Ok(data);
        }

        // 🔥 GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetail(long id)
        {
            var od = await _context.OrderDetails
                .Include(x => x.Product)
                .Include(x => x.Order)
                    .ThenInclude(o => o.User)
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Price,
                    x.Quantity,

                    Product = new
                    {
                        x.Product.Id,
                        x.Product.Name
                    },

                    User = new
                    {
                        x.Order.User.Id,
                        x.Order.User.Name
                    }
                })
                .FirstOrDefaultAsync();

            if (od == null) return NotFound();

            return Ok(od);
        }

        // 🔥 POST
        [HttpPost]
        public async Task<IActionResult> PostOrderDetail(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(detail);
        }

        // 🔥 PUT (FIX lỗi 500)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(long id, OrderDetail detail)
        {
            var existing = await _context.OrderDetails.FindAsync(id);

            if (existing == null)
                return NotFound("Không tìm thấy chi tiết đơn hàng");

            existing.ProductId = detail.ProductId;
            existing.Price = detail.Price;
            existing.Quantity = detail.Quantity;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔥 DELETE
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