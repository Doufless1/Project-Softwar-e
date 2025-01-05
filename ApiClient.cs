using System.Text.Json.Serialization;
using enums;

namespace Weather_App;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string baseAddress)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseAddress),
            Timeout = TimeSpan.FromMinutes(50)
        };
    }

    public async Task<Dictionary<FrontendReadyData, List<double>>> GetGraphDataAsync(string location)
    {
    //    var requestUri = $"api/graph/{location}";
       // _httpClient.BaseAddress(+ requestUri);
        try
        {
            var response = await _httpClient.GetAsync($"api/graph/{location}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                Console.WriteLine($"Error: {await response.Content.ReadAsStringAsync()}");
                return new Dictionary<FrontendReadyData, List<double>>();
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Received JSON:");
            Console.WriteLine(json);

            // Deserialize with proper options
            var options = new JsonSerializerOptions
            {
                // Use this if your enums are serialized as strings
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<Dictionary<FrontendReadyData, List<double>>>(json, options);
            return data ?? new Dictionary<FrontendReadyData, List<double>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new Dictionary<FrontendReadyData, List<double>>();
        }
    }

    public async Task<Dictionary<string, Dictionary<enums.AccesableData, double>>> GetGatewayDataAsync(string location)
    {
        var response = await _httpClient.GetAsync($"api/gateway/{location}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var rawData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(json);

        // Convert the inner dictionary keys to enums.AccesableData
        return rawData.ToDictionary(
            outer => outer.Key,
            outer => outer.Value.ToDictionary(
                innerKeyValue => Enum.TryParse<enums.AccesableData>(innerKeyValue.Key, out var parsedEnum)
                    ? parsedEnum
                    : throw new InvalidCastException($"Invalid enum value: {innerKeyValue.Key}"),
                innerKeyValue => innerKeyValue.Value
            )
        );
    }
}