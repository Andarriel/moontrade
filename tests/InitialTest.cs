using Client;
using Core;
using Shared;

namespace Tests;

public class Tests
{
    [Fact]
    public void InitialTest()
    {
        TradeStorage storage = new TradeStorage(10000);

        for (int i = 0; i < 25000; i++)
        {
            storage.Add(new Trade { Symbol = "BTCUSDT", Price = i, Quantity = 1, TimestampMs = i, Side = OrderSide.Buy });
        }

        Console.WriteLine(storage.CountFor("BTCUSDT"));
    }
}
