using HoThiBichNhung_2123110314.Data;
using HoThiBichNhung_2123110314.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HoThiBichNhung_2123110314.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class PaymentsController : ControllerBase
        {
            private readonly AppDbContext _context;

            public PaymentsController(AppDbContext context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Payment>>> Get()
                => await _context.Payments.ToListAsync();

            [HttpPost]
            public async Task<ActionResult<Payment>> Post(Payment payment)
            {
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                return Ok(payment);
            }
        }
}

