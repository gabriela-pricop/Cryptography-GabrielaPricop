using System.Text;

namespace Cryptography_lab1;

public class CaesarCipherPermutation
{
    private const int N = 31;
    private const int MinKey = 1;
    private const int MaxKey = N - 1;
    private const int MinKeywordLength = 7;

    private static readonly char[] Upper =
    {
        'A', 'Ă', 'Â', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'Î', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'Ș', 'T', 'Ț', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    private static readonly Dictionary<char, int> CodeOf = BuildCodeTable();

    private static Dictionary<char, int> BuildCodeTable()
    {
        var table = new Dictionary<char, int>();
        for (int i = 0; i < N; i++)
        {
            table[Upper[i]] = i;
        }
        return table;
    }

    private static char NormalizeCedilla(char c)
    {
        switch (c)
        {
            case '\u015E': return '\u0218'; // Ş -> Ș
            case '\u0162': return '\u021A'; // Ţ -> Ț
            default: return c;
        }
    }

    private static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    // Validates the input, converts it to uppercase and (optionally) removes spaces.
    // On failure, 'rejected' holds the first invalid character.
    private static bool TryPrepare(string input, bool allowSpaces, out string prepared, out char rejected)
    {
        var sb = new StringBuilder();
        rejected = '\0';

        foreach (char raw in input)
        {
            if (allowSpaces && raw == ' ')
            {
                continue;
            }

            char c = NormalizeCedilla(char.ToUpperInvariant(raw)); // uppercase first, then fix cedilla
            if (!CodeOf.TryGetValue(c, out int code))
            {
                rejected = raw;
                prepared = string.Empty;
                return false;
            }

            sb.Append(Upper[code]); // already uppercase
        }

        prepared = sb.ToString();
        return true;
    }

    /// Builds the permuted alphabet: the distinct letters of the keyword first
    /// (first occurrence only), followed by the remaining letters in natural order.
    private static char[] BuildPermutedAlphabet(string keyword)
    {
        var used = new bool[N];
        var permuted = new List<char>(N);

        foreach (char c in keyword)
        {
            int code = CodeOf[c];
            if (!used[code])
            {
                used[code] = true;
                permuted.Add(Upper[code]);
            }
        }

        for (int i = 0; i < N; i++)
        {
            if (!used[i])
            {
                permuted.Add(Upper[i]);
            }
        }

        return permuted.ToArray();
    }

    // For every standard code (0..30) returns the position of that letter in the permuted alphabet.
    private static int[] BuildPositionTable(char[] permuted)
    {
        var position = new int[N];
        for (int p = 0; p < N; p++)
        {
            position[CodeOf[permuted[p]]] = p;
        }
        return position;
    }

    //c = P[(pos(x) + k) mod n]  or  m = P[(pos(y) - k) mod n].
    private static string Transform(string text, int key, char[] permuted, int[] position, bool encrypt)
    {
        int shift = encrypt ? key : -key;
        var sb = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            int x = position[CodeOf[c]];            
            sb.Append(permuted[Mod(x + shift, N)]);
        }

        return sb.ToString();
    }

    private static void PrintAlphabets(char[] permuted)
    {
        Console.WriteLine();
        Console.WriteLine("Permuted alphabet (code = position in the permuted alphabet):");

        for (int start = 0; start < N; start += 16)
        {
            int end = Math.Min(start + 16, N);

            Console.Write("Code".PadRight(10) + ":");
            for (int i = start; i < end; i++) Console.Write(i.ToString().PadLeft(3));
            Console.WriteLine();

            Console.Write("Standard".PadRight(10) + ":");
            for (int i = start; i < end; i++) Console.Write(Upper[i].ToString().PadLeft(3));
            Console.WriteLine();

            Console.Write("Permuted".PadRight(10) + ":");
            for (int i = start; i < end; i++) Console.Write(permuted[i].ToString().PadLeft(3));
            Console.WriteLine();
            Console.WriteLine();
        }

        Console.WriteLine("Permuted alphabet: " + string.Join(" ", permuted));
    }

    private static string Prompt(string message)
    {
        Console.Write(message);
        string line = Console.ReadLine();
        if (line == null)
        {
            Environment.Exit(0);
        }
        return line;
    }

    private static int ReadShiftKey()
    {
        while (true)
        {
            string input = Prompt($"Enter key 1 - the shift k (integer from {MinKey} to {MaxKey}): ").Trim();

            if (!int.TryParse(input, out int key))
            {
                Console.WriteLine($"Error: \"{input}\" is not an integer. Key 1 must be a whole number from {MinKey} to {MaxKey}.");
                continue;
            }

            if (key < MinKey || key > MaxKey)
            {
                Console.WriteLine($"Error: key {key} is out of range. The allowed range is {MinKey}..{MaxKey} inclusive.");
                continue;
            }

            return key;
        }
    }

    private static string ReadKeyword()
    {
        while (true)
        {
            string input = Prompt($"Enter key 2 - the keyword (Romanian letters only, at least {MinKeywordLength} characters): ").Trim();

            if (!TryPrepare(input, false, out string keyword, out char rejected))
            {
                string shown = rejected == ' ' ? "space" : $"'{rejected}'";
                Console.WriteLine($"Error: the character {shown} is not allowed in the keyword. " +
                                  "Use only letters of the Romanian alphabet (A-Z, Ă, Â, Î, Ș, Ț), without spaces.");
                continue;
            }

            if (keyword.Length < MinKeywordLength)
            {
                Console.WriteLine($"Error: the keyword has {keyword.Length} character(s). " +
                                  $"The minimum length is {MinKeywordLength}.");
                continue;
            }

            return keyword;
        }
    }

    private static string ReadText(bool encrypt)
    {
        string label = encrypt ? "message" : "ciphertext";

        while (true)
        {
            string input = Prompt($"Enter the {label}: ");

            if (!TryPrepare(input, true, out string prepared, out char rejected))
            {
                Console.WriteLine($"Error: the character '{rejected}' is not allowed. " +
                                  "Only letters of the Romanian alphabet (A-Z, Ă, Â, Î, Ș, Ț) and spaces are accepted.");
                continue;
            }

            if (prepared.Length == 0)
            {
                Console.WriteLine($"Error: the {label} is empty. Enter at least one letter.");
                continue;
            }

            return prepared;
        }
    }

    public static void Run()
    {
        Console.WriteLine("=== Caesar cipher with a permutation - Romanian alphabet (n = 31) ===");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1 - Encrypt");
            Console.WriteLine("2 - Decrypt");
            Console.WriteLine("0 - Exit");
            string choice = Prompt("Choose the operation: ").Trim();

            if (choice == "0")
            {
                break;
            }

            if (choice != "1" && choice != "2")
            {
                Console.WriteLine("Error: please enter 1 (encrypt), 2 (decrypt) or 0 (exit).");
                continue;
            }

            bool encrypt = choice == "1";

            int key = ReadShiftKey();
            string keyword = ReadKeyword();

            char[] permuted = BuildPermutedAlphabet(keyword);
            int[] position = BuildPositionTable(permuted);

            Console.WriteLine($"Keyword (uppercase): {keyword}");
            PrintAlphabets(permuted);

            string text = ReadText(encrypt);

            Console.WriteLine($"Prepared text : {text}");
            Console.WriteLine(encrypt
                ? $"Ciphertext    : {Transform(text, key, permuted, position, true)}"
                : $"Decrypted text: {Transform(text, key, permuted, position, false)}");
        }
    }
}