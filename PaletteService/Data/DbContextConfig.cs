/* DbContextConfig.cs
*
* @description: Configures the DbContext for the PaletteService
*
* @author: Andrew Bazen <github.com/AndrewBazen>
* @version: 1.0
* @date-created: 2025-05-03
* @date-modified: 2025-05-03
*
* @see: PaletteDbContext.cs
* @see: PalettesController.cs
*/
using Microsoft.EntityFrameworkCore;

namespace PaletteService.Data;

/* @class: DbContextConfig
*
* @description: Configures the DbContext for the PaletteService
* @date-created: 2025-05-03
* @date-modified: 2025-05-03
*/
public static class DbContextConfig
{
    /* @method: Configure
    *
    * @description: Configures the DbContext for the PaletteService
    * @param: IServiceCollection services
    * @param: IConfiguration config
    * @return: void
    */
    public static void Configure(IServiceCollection services, IConfiguration config)
    {
        var provider = config["Database:Provider"] ?? "PostgreSQL";
        var connStr = config.GetConnectionString(provider) ?? throw new Exception(
            $"Connection string not found for provider: {provider}");
        services.AddDbContext<PaletteDbContext>(options =>
        {
            if (provider == "PostgreSQL")
                options.UseNpgsql(connStr);
            else
                throw new Exception($"Unsupported database provider: {provider}");
        });
    }
}

