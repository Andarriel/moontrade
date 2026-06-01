using Shared;

namespace Client;

public sealed class Trade
{
    public required string Symbol { get; init; }
    public decimal Price { get; init; }
    public decimal Quantity { get; init; }
    public long TimestampMs { get; init; }
    public OrderSide Side { get; init; }
}
