using Client;

namespace Core;

public sealed class TradeStorage
{
    private readonly object _lock = new object();
    private readonly Dictionary<string, Queue<Trade>> store = new Dictionary<string, Queue<Trade>>();

    private readonly int _capacity;

    public TradeStorage(int capacity)
    {
        this._capacity = capacity;
    }

    public void Add(Trade trade)
    {
        lock (_lock)
        {
            if (!store.TryGetValue(trade.Symbol, out Queue<Trade>? queue))
            {
                queue = new Queue<Trade>();
                store[trade.Symbol] = queue;
            }

            queue.Enqueue(trade);
        }
    }

    public int CountFor(string symbol)
    {
        lock (_lock)
        {
            if (store.TryGetValue(symbol, out Queue<Trade>? queue))
            {
                return queue.Count;
            }

            return 0;
        }
    }

    public void Cleanup()
    {
        lock (_lock)
        {
            foreach(KeyValuePair<string,Queue<Trade>> entry in store)
            {
                Queue<Trade> queue = entry.Value;

                while(queue.Count > _capacity)
                {
                    queue.Dequeue();
                }
            }
        }
    }
}
