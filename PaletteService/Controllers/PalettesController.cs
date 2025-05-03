using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaletteService.Data;
using PaletteService.Models;


namespace PaletteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    internal class PalettesController : ControllerBase
    {
        private readonly PaletteDBContext _context;

        public PalettesController(PaletteDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => 
            Ok(await _context.Palettes.ToListAsync());

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug) {
            if (string.IsNullOrEmpty(slug)) return BadRequest("Slug cannot be null or empty");
            var palette = await _context.Palettes.FirstOrDefaultAsync(p => p.Slug == slug);
            return palette is null ? NotFound() : Ok(palette);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Palette palette) {
            if (palette is null) return BadRequest("Palette cannot be null");
            if (string.IsNullOrEmpty(palette.Slug)) return BadRequest("Slug cannot be null or empty");
            _context.Palettes.Add(palette);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBySlug), new { slug = palette.Slug }, palette);
        }
    }
}
