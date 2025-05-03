using PocketSpriteLib.Services.PaletteService;
using System.Diagnostics;

namespace PocketSpriteTest
{
    public class APITest
    {
        [Fact]
        public void HttpClientResponseTest() {
            ApiService apiService = new ApiService();

            var response = apiService.GetPalettesAsync();

            Assert.NotNull(response);
            foreach (Palette palette in response.Result.ToList()) {
                Assert.NotNull(palette);
                Assert.NotNull(palette.Id);
                Assert.NotNull(palette.Author);
                Assert.NotNull(palette.Colors);
                Assert.True(palette.Colors.Count > 0);
                Debug.WriteLine($"Palette Name: {palette.Id}");
                Debug.WriteLine($"Palette Author: {palette.Author}");
                Debug.WriteLine($"Palette Colors: {string.Join(", ", palette.Colors)}");
            }   
        }
    }
}
