namespace Cryptography_lab2;

public class FrequencyAnalyzer
{
    public static Dictionary<char, int> CountLetters(string text)
    {
        var counts = new Dictionary<char, int>();
        for (char letter = 'A'; letter <= 'Z'; letter++)
        {
            counts[letter] = 0;
        }

        foreach (char raw in text)
        {
            char c = char.ToUpperInvariant(raw);
            if (c >= 'A' && c <= 'Z')
            {
                counts[c]++;
            }
        }

        return counts;
    }

    // Converts the letter counts to percentages of the total number of letters.
    public static Dictionary<char, double> ToPercent(Dictionary<char, int> counts)
    {
        int total = counts.Values.Sum();
        var percent = new Dictionary<char, double>();

        foreach (var pair in counts)
        {
            percent[pair.Key] = total == 0 ? 0.0 : 100.0 * pair.Value / total;
        }

        return percent;
    }

    // Orders the letters by the given value, descending; ties are broken alphabetically
    public static List<char> SortDescending<T>(Dictionary<char, T> values) where T : System.IComparable<T>
    {
        return values
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key)
            .Select(pair => pair.Key)
            .ToList();
    }
}