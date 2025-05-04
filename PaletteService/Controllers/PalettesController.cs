/* PalettesController.cs
*
* @description: Controller for the PaletteService
*
* @author: Andrew Bazen <github.com/AndrewBazen>
* @version: 1.0
* @date-created: 2025-05-03
* @date-modified: 2025-05-03
*
* @see: PaletteDbContext.cs
* @see: Palette.cs
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaletteService.Data;
using PaletteService.Models;

namespace PaletteService.Controllers
{
    /* @class: PalettesController
    *
    * @description: Controller for the PaletteService
    * @date-created: 2025-05-03
    * @date-modified: 2025-05-03
    */
    [ApiController]
    [Route("api/[controller]")]
    public class PalettesController : ControllerBase
    {
        private readonly PaletteDbContext _context;
        
        /* @constructor: PalettesController
        *
        * @description: Constructor for the PalettesController
        * @param: PaletteDbContext context
        * @return: void
        */
        public PalettesController(PaletteDbContext context) {
            _context = context;
        }

        /* @method: GetRoot
        *
        * @description: Gets the root endpoint
        * @return: IActionResult
        */
        [HttpGet("/")]
        public IActionResult GetRoot() => 
            Ok(new { message = "Palette Service API is running" });

        /* @method: GetAll
        *
        * @description: Gets all palettes
        * @return: IActionResult
        */
        [HttpGet]
        public async Task<IActionResult> GetAll() => 
            Ok(await _context.Palettes.ToListAsync());

        /* @method: GetBySlug
        *
        * @description: Gets a palette by slug
        * @param: string slug
        * @return: IActionResult
        */
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug) {
            if (string.IsNullOrEmpty(slug)) return BadRequest("Slug cannot be null or empty");
            var palette = await _context.Palettes.FirstOrDefaultAsync(p => p.Slug == slug);
            return palette is null ? NotFound() : Ok(palette);
        }

        /* @method: Create
        *
        * @description: Creates a new palette
        * @param: Palette palette
        * @return: IActionResult
        */
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
