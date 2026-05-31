namespace Cli;

public sealed class PairSelector
{
    public List<string> Select(string userInput, List<string> availableSymbols)
    {
        char[] separators = new char[] { ',', ' ' };
        string[] pairs = userInput.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        HashSet<string> valid = new HashSet<string>(availableSymbols);
        List<string> binancePairs = new List<string>();

        foreach (string pair in pairs)
        {
            string normalized = pair.Replace("/", "").Trim().ToUpperInvariant();

            if (valid.Contains(normalized))
            {
                if (!binancePairs.Contains(normalized))
                {
                    binancePairs.Add(normalized);
                }
            }
            else
            {
                Console.WriteLine($"Unknown pair, skipping: {pair}");
            }
        }

        return binancePairs;
    }
}
