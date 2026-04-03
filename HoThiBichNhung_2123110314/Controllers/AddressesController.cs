using HoThiBichNhung_2123110314.Data;
using HoThiBichNhung_2123110314.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoThiBichNhung_2123110314.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> Get()
        {
            return await _context.Addresses.ToListAsync();
        }

        // POST: api/addresses
        [HttpPost]
        public async Task<ActionResult<Address>> Post(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return Ok(address);
        }

        // DELETE: api/addresses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var addr = await _context.Addresses.FindAsync(id);

            if (addr == null)
                return NotFound();

            _context.Addresses.Remove(addr);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}