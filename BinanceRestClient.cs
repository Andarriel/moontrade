using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

public sealed class BinanceRestClient
{
    public List<string> GetSymbols()
    {
        HttpClient client = new HttpClient();
        string json = client.GetStringAsync("https://api.binance.com/api/v3/exchangeInfo").GetAwaiter().GetResult();

        using JsonDocument document = JsonDocument.Parse(json);

    }
}
