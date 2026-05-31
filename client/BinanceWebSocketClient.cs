using Shared;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Client;

public sealed class BinanceWebSocketClient
{
    private readonly string _symbol;
    private readonly Action<Trade> _onTrade;

    public BinanceWebSocketClient(string symbol, Action<Trade> onTrade)
    {
        _symbol = symbol;
        _onTrade = onTrade;
    }

    public void Run()
    {
        ClientWebSocket ws = new ClientWebSocket();
        Uri uri = new Uri($"wss://stream.binance.com:9443/ws/{_symbol.ToLower()}@trade");

        ws.ConnectAsync(uri, CancellationToken.None).GetAwaiter().GetResult();

        byte[] buffer = new byte[8192];

        while (ws.State == WebSocketState.Open)
        {
            using MemoryStream ms = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).GetAwaiter().GetResult();
                if(result.MessageType == WebSocketMessageType.Close)
                {
                    return;
                }
                ms.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);

            string message = Encoding.UTF8.GetString(ms.ToArray());

            Trade trade = ParseTrade(message);
            _onTrade(trade);
        }
    }

    private Trade ParseTrade(string message)
    {
        using JsonDocument doc = JsonDocument.Parse(message);
        JsonElement root = doc.RootElement;
        
        Trade trade = new Trade
        {
            Symbol = root.GetProperty("s").GetString()!,
            Price = decimal.Parse(root.GetProperty("p").GetString()!, CultureInfo.InvariantCulture),
            Quantity = decimal.Parse(root.GetProperty("q").GetString()!, CultureInfo.InvariantCulture),
            TimestampMs = root.GetProperty("T").GetInt64(),
            Side = (root.GetProperty("m").GetBoolean() ? OrderSide.Sell : OrderSide.Buy)
        };

        return trade;
    }
}

