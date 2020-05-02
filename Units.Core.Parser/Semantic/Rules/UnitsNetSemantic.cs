
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Units.Core.Parser.State;

namespace Units.Core.Parser.Semantic.Rules
{
    [Semantic("UnitsNet")]
    public class UnitsNetSemantic : SemanticAbstract
    {
        public static readonly string AppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnitsCore");
        public static readonly string CacheDirectory = Path.Combine(AppDir, ".cache");
        private static readonly HttpClient _httpClient = new HttpClient() {
            BaseAddress = new Uri("https://raw.githubusercontent.com/angularsen/UnitsNet/master/Common/UnitDefinitions/")
        };
        public UnitsNetSemantic(ParserState state) : base(state)
        {
            if (!Directory.Exists(AppDir))
                Directory.CreateDirectory(AppDir);
        }
        [Semantic("Dimension")]
        public void GetDimension(string dimensionName)
        {
            
            GetDimensionAsync(dimensionName).GetAwaiter().GetResult();
        }
        public async Task GetDimensionAsync(string dimensionName)
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    Directory.CreateDirectory(CacheDirectory);
                }
                var dimensionPath = Path.Combine(CacheDirectory, $"{dimensionName}.json");
                Metadata.UnitsNetDescription description;
                if (!File.Exists(dimensionPath))
                {
                    var res = await _httpClient.GetAsync($"{dimensionName}.json");
                    if (!res.IsSuccessStatusCode)
                        throw new HandleException($"Invalid UnitsNet name {dimensionName}", 1101);
                    using var fs = new FileStream(dimensionPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    await res.Content.CopyToAsync(fs);
                    fs.Position = 0;
                    fs.Seek(0, SeekOrigin.Begin);
                    description = await System.Text.Json.JsonSerializer.DeserializeAsync<Metadata.UnitsNetDescription>(fs);
                }
                else
                {
                    using var fs = new FileStream(dimensionPath, FileMode.OpenOrCreate, FileAccess.Read);
                    fs.Seek(0, SeekOrigin.Begin);
                    description = await System.Text.Json.JsonSerializer.DeserializeAsync<Metadata.UnitsNetDescription>(fs);
                }
                _state.AddUnitsNetDescription(description);
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message, 1537);
            }
        }
        [Semantic("All")]
        public void GetAllDimensions()
        {
            GetAllDimensionsAsync().GetAwaiter().GetResult();
        }
        public async Task GetAllDimensionsAsync()
        {
            foreach(var u in Metadata.UnitsNetAllUnits.UnitsNetUnits.Batched(4))
            {
                await Task.WhenAll(u.Select(i => GetDimensionAsync(i)));
            }
        }
    }
}
