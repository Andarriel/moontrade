using Client;

namespace Core;

public sealed class SubscriptionManager
{
    private readonly Action<Trade> _onTrade;
    private readonly List<Thread> _threads = new List<Thread>();

    public SubscriptionManager(Action<Trade> onTrade)
    {
        _onTrade = onTrade;
    }

    public void Start(List<string> symbols)
    {
        foreach (string symbol in symbols)
        {
            BinanceWebSocketClient client = new BinanceWebSocketClient(symbol, _onTrade);
            
            Thread thread = new Thread(client.Run);
            
            thread.IsBackground = true;
            thread.Name = $"ws-{symbol}";

            _threads.Add(thread);
            thread.Start();
        }
    }
}