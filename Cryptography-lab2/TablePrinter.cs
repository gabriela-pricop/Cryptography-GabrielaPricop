namespace Cryptography_lab2;

public static class TablePrinter
{
    private const int LabelWidth = 9;
    private const int CellWidth = 6;
    private const int LettersPerBlock = 13;

    public static void Print(string title, IList<char> letters, IList<string> values, string valueLabel)
    {
        Console.WriteLine(title);
        Console.WriteLine();

        for (int start = 0; start < letters.Count; start += LettersPerBlock)
        {
            int end = Math.Min(start + LettersPerBlock, letters.Count);
            string separator = BuildSeparator(end - start);

            Console.WriteLine(separator);
            Console.WriteLine(BuildRow("Letter", start, end, i => letters[i].ToString()));
            Console.WriteLine(separator);
            Console.WriteLine(BuildRow(valueLabel, start, end, i => values[i]));
            Console.WriteLine(separator);
        }

        Console.WriteLine();
    }

    private static string BuildSeparator(int cells)
    {
        string line = "+" + new string('-', LabelWidth) + "+";
        for (int i = 0; i < cells; i++)
        {
            line += new string('-', CellWidth) + "+";
        }
        return line;
    }

    private static string BuildRow(string label, int start, int end, Func<int, string> cell)
    {
        string line = "|" + Center(label, LabelWidth) + "|";
        for (int i = start; i < end; i++)
        {
            line += Center(cell(i), CellWidth) + "|";
        }
        return line;
    }

    private static string Center(string text, int width)
    {
        int space = width - text.Length;
        int left = space / 2;
        return new string(' ', left) + text + new string(' ', space - left);
    }
}
