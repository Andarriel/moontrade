using System.Text.Json;

namespace Client;

public sealed class BinanceRestClient
{
    private static readonly HttpClient client = new();
    private static readonly string url = "https://api.binance.com/api/v3/exchangeInfo" +
                                         "?permissions=SPOT&symbolStatus=TRADING&showPermissionSets=false";
    public List<string> GetSymbols()
    {
        

        string json = client.GetStringAsync(url).GetAwaiter().GetResult();

        using JsonDocument document = JsonDocument.Parse(json);

        JsonElement root = document.RootElement;
        JsonElement symbols = root.GetProperty("symbols");

        List<string> result = new List<string>();

        foreach (JsonElement symbol in symbols.EnumerateArray())
        {
            string name = symbol.GetProperty("symbol").GetString() ?? string.Empty;
            result.Add(name);
        }

        return result;
    }
}
