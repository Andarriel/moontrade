using Cli;
using Client;
using Core;
using Shared.win64;

if (OperatingSystem.IsWindows())
{
    WindowsConsole.DisableQuickEdit();
}

BinanceRestClient rest = new BinanceRestClient();
List<string> symbols = rest.GetSymbols();

string userInput;
if (args.Length > 0)
{
    userInput = string.Join(" ", args);
} 
else
{
    Console.Write("Choose pairs: ");
    userInput = Console.ReadLine() ?? string.Empty;
}

PairSelector selector = new PairSelector();

List<string> selectedPairs = selector.Select(userInput, symbols);

if (selectedPairs.Count == 0)
{
    Console.WriteLine("No valid pairs selected. Exiting.");
    return;
}

TradeStorage storage = new TradeStorage(10000);

ConsolePrinter printer = new ConsolePrinter();
printer.Start();

Action<Trade> onTrade = trade =>
{
    storage.Add(trade);
    printer.Enqueue(trade);
};

SubscriptionManager manager = new SubscriptionManager(onTrade);
manager.Start(selectedPairs);

CleanupWorker cleanup = new CleanupWorker(storage, 60_000);
cleanup.Start();

Thread.Sleep(Timeout.Infinite);