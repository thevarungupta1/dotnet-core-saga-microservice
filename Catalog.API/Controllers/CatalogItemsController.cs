using Catalog.API.Database;
using Catalog.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogItemsController : ControllerBase
    {
        private readonly CatalogContext _context;

        public CatalogItemsController(CatalogContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogItem>>> GetCatalogItems()
        {
            var catalogItem = await _context.CatalogItems.ToListAsync();
            return Ok(catalogItem);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CatalogItem>>> GetCatalogItem(int id)
        {
            var catalogItem = await _context.CatalogItems.FindAsync(id);
            if(catalogItem == null)
            {
                return NotFound();
            }
            return Ok(catalogItem);
        }

        [HttpPost]
        public async Task<ActionResult<CatalogItem>> PostCatalogItem(CatalogItem catalogItem)
        {
            _context.CatalogItems.Add(catalogItem); 
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCatalogItems", new { id = catalogItem.Id }, catalogItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatalogItem(int id, CatalogItem catalogItem)
        {
            if(id != catalogItem.Id)
            {
                return BadRequest();
            }
            _context.Entry(catalogItem).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogItem(int id)
        {
            var catalogItem = await _context.CatalogItems.FindAsync(id);
            if(catalogItem == null)
            {
                return NotFound();
            }
            _context.CatalogItems.Remove(catalogItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
