using Client;
using Core;
using Shared;

namespace Tests;

public class TradeStorageTests
{
    private static Trade MakeTrade(string symbol, decimal price)
    {
        return new Trade
        {
            Symbol = symbol,
            Price = price,
            Quantity = 1m,
            TimestampMs = 0,
            Side = OrderSide.Buy
        };
    }

    [Fact]
    public void AddBullshit()
    {
        TradeStorage storage = new TradeStorage(10000);

        storage.Add(MakeTrade("BTCUSDT", 1));
        storage.Add(MakeTrade("BTCUSDT", 2));
        storage.Add(MakeTrade("BTCUSDT", 3));

        Assert.Equal(3, storage.CountFor("BTCUSDT"));
    }

    [Fact]
    public void UnknownSymbol()
    {
        TradeStorage storage = new TradeStorage(10000);

        Assert.Equal(0, storage.CountFor("ETHUSDT"));
    }

    [Fact]
    public void CleanupTrimsEachPairToCapacity()
    {
        TradeStorage storage = new TradeStorage(100);

        for (int i = 0; i < 250; i++)
        {
            storage.Add(MakeTrade("BTCUSDT", i));
        }

        Assert.Equal(250, storage.CountFor("BTCUSDT"));
        storage.Cleanup();
        Assert.Equal(100, storage.CountFor("BTCUSDT"));
    }
    
    [Fact]
    public void StorageKeepsPairsIndependent()
    {
        TradeStorage storage = new TradeStorage(100);

        storage.Add(MakeTrade("BTCUSDT", 1));
        storage.Add(MakeTrade("ETHUSDT", 1));
        storage.Add(MakeTrade("ETHUSDT", 2));

        Assert.Equal(1, storage.CountFor("BTCUSDT"));
        Assert.Equal(2, storage.CountFor("ETHUSDT"));
    }

    [Fact]
    public void AddIsThreadSafeUnderConcurrentWriters()
    {
        TradeStorage storage = new TradeStorage(1_000_000);
        List<Thread> threads = new List<Thread>();

        for (int t = 0; t < 4; t++)
        {
            Thread thread = new Thread(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    storage.Add(MakeTrade("BTCUSDT", i));
                }
            });
            threads.Add(thread);
            thread.Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        Assert.Equal(40000, storage.CountFor("BTCUSDT"));
    }
}