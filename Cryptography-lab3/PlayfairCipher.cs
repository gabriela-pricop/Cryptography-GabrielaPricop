using System.Text;

namespace Cryptography_lab3;

public class PlayfairCipher : ICipher
{
    // 31 letters, J is replaced by I -> 30 letters -> 5x6 matrix
    private const int Rows = 5;
    private const int Cols = 6;
 
    public string Name => "Playfair";
 
    public string Encrypt(string text, string key)
    {
        char[,] matrix = BuildMatrix(key);
        string digraphs = ToDigraphs(text.Replace('J', 'I'));
        return Transform(digraphs, matrix, 1);
    }
 
    public string Decrypt(string text, string key)
    {
        if (text.Length % 2 != 0)
        {
            throw new ArgumentException(
                $"the ciphertext has {text.Length} letters; Playfair needs an even number of letters (pairs).");
        }
 
        if (text.Contains('J'))
        {
            throw new ArgumentException(
                "the ciphertext contains 'J', which is not in the Playfair matrix (J is replaced by I).");
        }
 
        char[,] matrix = BuildMatrix(key);
        return Transform(text, matrix, -1);
    }
 
    public string? KeyInfo(string key)
    {
        char[,] matrix = BuildMatrix(key);
        Console.WriteLine();
        var sb = new StringBuilder("Playfair matrix (J is replaced by I):" + Environment.NewLine);
 
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                sb.Append(matrix[r, c].ToString().PadLeft(3));
            }
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }
 
    // Key letters first (without repetitions), then the rest of the alphabet
    private static char[,] BuildMatrix(string key)
    {
        var used = new HashSet<char>();
        var order = new List<char>();
 
        foreach (char c in key.Replace('J', 'I'))
        {
            if (used.Add(c))
            {
                order.Add(c);
            }
        }
 
        foreach (char c in Alphabet.Letters)
        {
            if (c != 'J' && used.Add(c))
            {
                order.Add(c);
            }
        }
 
        var matrix = new char[Rows, Cols];
        for (int i = 0; i < order.Count; i++)
        {
            matrix[i / Cols, i % Cols] = order[i];
        }
 
        return matrix;
    }
 
    // Splits the text into pairs; a filler letter separates double letters and completes an odd end
    private static string ToDigraphs(string text)
    {
        var sb = new StringBuilder();
        int i = 0;
 
        while (i < text.Length)
        {
            char a = text[i];
 
            if (i + 1 < text.Length && text[i + 1] != a)
            {
                sb.Append(a).Append(text[i + 1]);
                i += 2;
            }
            else
            {
                sb.Append(a).Append(Filler(a));
                i += 1;
            }
        }
 
        return sb.ToString();
    }
 
    private static char Filler(char a)
    {
        return a == 'X' ? 'Q' : 'X';
    }
 
    private static (int row, int col) Find(char[,] matrix, char letter)
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (matrix[r, c] == letter)
                {
                    return (r, c);
                }
            }
        }
 
        throw new ArgumentException($"the letter '{letter}' is not in the Playfair matrix.");
    }
 
    // step = +1 for encryption, -1 for decryption
    private static string Transform(string text, char[,] matrix, int step)
    {
        var sb = new StringBuilder(text.Length);
 
        for (int i = 0; i < text.Length; i += 2)
        {
            var (r1, c1) = Find(matrix, text[i]);
            var (r2, c2) = Find(matrix, text[i + 1]);
 
            if (r1 == r2)
            {
                // same row: neighbour to the right (left when decrypting)
                c1 = Alphabet.Mod(c1 + step, Cols);
                c2 = Alphabet.Mod(c2 + step, Cols);
            }
            else if (c1 == c2)
            {
                // same column: neighbour below (above when decrypting)
                r1 = Alphabet.Mod(r1 + step, Rows);
                r2 = Alphabet.Mod(r2 + step, Rows);
            }
            else
            {
                // rectangle: exchange the columns
                (c1, c2) = (c2, c1);
            }
 
            sb.Append(matrix[r1, c1]).Append(matrix[r2, c2]);
        }
 
        return sb.ToString();
    }
}
 