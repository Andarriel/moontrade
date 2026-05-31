namespace Core;
public sealed class CleanupWorker
{
    private readonly TradeStorage _storage;
    private readonly int _intervalMs;

    public CleanupWorker(TradeStorage storage, int intervalMs)
    {
        _storage = storage;
        _intervalMs = intervalMs;
    }

    public void Run()
    {
        while (true) 
        {
            Thread.Sleep(_intervalMs);
            _storage.Cleanup();
        }
    }

    public void Start()
    {
        Thread thread = new Thread(Run);
        thread.IsBackground = true;
        thread.Name = "cleanup";
        thread.Start();
    }
}

