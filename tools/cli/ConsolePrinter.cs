using Client;
using Shared;
using System.Collections.Concurrent;

namespace Cli;

public sealed class ConsolePrinter
{
    private readonly BlockingCollection<Trade> _queue = new BlockingCollection<Trade>();

    public void Enqueue(Trade trade)
    {
        _queue.Add(trade);
    }

    public void Start()
    {
        Thread thread = new Thread(Run);
        thread.IsBackground = true;
        thread.Name = "printer";
        thread.Start();
    }

    private void Run()
    {
        foreach (Trade trade in _queue.GetConsumingEnumerable())
        {
            Console.ForegroundColor = trade.Side == OrderSide.Buy ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{trade.Symbol} {trade.Side} {trade.Price} {trade.Quantity} {trade.TimestampMs}");
            Console.ResetColor();
        }
    }
}
