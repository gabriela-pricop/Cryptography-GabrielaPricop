using System.Text;

namespace Cryptography_lab3;


public static class Alphabet
{
    public const int Size = 31;

    public static readonly char[] Letters =
    {
        'A', 'Ă', 'Â', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'Î', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'Ș', 'T', 'Ț', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    private static readonly Dictionary<char, int> Codes = BuildCodes();

    private static Dictionary<char, int> BuildCodes()
    {
        var table = new Dictionary<char, int>();
        for (int i = 0; i < Size; i++)
        {
            table[Letters[i]] = i;
        }

        return table;
    }

    public static int CodeOf(char c)
    {
        return Codes[c];
    }

    public static bool Contains(char c)
    {
        return Codes.ContainsKey(c);
    }

    // Always returns a value in 0..n-1, even for negative a
    public static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    public static bool IsLatinLetter(char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    // Replace the cedilla variants of S and T with the comma-below letters
    private static char NormalizeCedilla(char c)
    {
        switch (c)
        {
            case '\u015E': return '\u0218';
            case '\u0162': return '\u021A';
            default: return c;
        }
    }

    // Removes spaces, converts to uppercase and checks every character.
    // allowRomanian = false: only A-Z / a-z (plaintext).
    // allowRomanian = true: the full 31-letter alphabet (ciphertext may contain Ă, Â, Î, Ș, Ț).
    public static bool TryPrepare(string input, bool allowRomanian, out string prepared, out char rejected)
    {
        var sb = new StringBuilder();
        rejected = '\0';

        foreach (char raw in input)
        {
            if (raw == ' ')
            {
                continue;
            }

            char c = NormalizeCedilla(char.ToUpperInvariant(raw));
            bool valid = allowRomanian ? Contains(c) : IsLatinLetter(raw);

            if (!valid)
            {
                rejected = raw;
                prepared = string.Empty;
                return false;
            }

            sb.Append(c);
        }

        prepared = sb.ToString();
        return true;
    }
}