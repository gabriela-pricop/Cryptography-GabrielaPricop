using System.Text;

namespace Cryptography_lab1;

public class CaesarCipher
{
    //alphabet size
    private const int N = 31;    
    //k = 1 to 30 inclusive
    private const int MinKey = 1;   
    private const int MaxKey = N - 1; 
    
    private static readonly char[] Upper =
    {
        'A', 'Ă', 'Â', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'Î', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'Ș', 'T', 'Ț', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    //table 2 recreation: maps every accepted letter to its code 0..30.
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

    //cedilla edge case handling
    private static char NormalizeCedilla(char c)
    {
        switch (c)
        {
            //unicode code point
            case '\u015E': return '\u0218'; 
            case '\u0162': return '\u021A'; 
            default: return c;
        }
    }

    //modulo that always returns a positive character
    private static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }

    /// Validates the text, converts it to uppercase and removes spaces.
    /// On failure, 'rejected' holds the first invalid character.
    private static bool TryPrepareText(string input, out string prepared, out char rejected)
    {
        var sb = new StringBuilder();
        rejected = '\0';

        foreach (char raw in input)
        {
            if (raw == ' ')
            {
                //skipping the space, that is how they are removed
                continue; 
            }
            //uppercase first then cedilla
            char c = NormalizeCedilla(char.ToUpperInvariant(raw)); 
            if (!CodeOf.TryGetValue(c, out int code))
            {
                rejected = raw;
                prepared = string.Empty;
                return false;
            }

            sb.Append(Upper[code]);
        }

        prepared = sb.ToString();
        return true;
    }

    //c = (x + k) mod n  or  m = (y - k) mod n, applied letter by letter
    private static string Transform(string text, int key, bool encrypt)
    {
        int shift = encrypt ? key : -key;
        var sb = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            int x = CodeOf[c];
            sb.Append(Upper[Mod(x + shift, N)]);
        }

        return sb.ToString();
    }

    private static string Prompt(string message)
    {
        Console.Write(message);
        string line = Console.ReadLine();
        if (line == null)
        {
            Environment.Exit(0); // input stream closed
        }
        return line;
    }

    private static int ReadShiftKey()
    {
        while (true)
        {
            string input = Prompt($"Enter the key k (integer from {MinKey} to {MaxKey}): ").Trim();

            if (!int.TryParse(input, out int key))
            {
                Console.WriteLine($"Error: \"{input}\" is not an integer. The key must be a whole number from {MinKey} to {MaxKey}.");
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

    private static string ReadText(bool encrypt)
    {
        string label = encrypt ? "message" : "ciphertext";

        while (true)
        {
            string input = Prompt($"Enter the {label}: ");

            if (!TryPrepareText(input, out string prepared, out char rejected))
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
        Console.WriteLine("=== Caesar cipher - Romanian alphabet (n = 31) ===");
        Console.WriteLine("Alphabet: " + string.Join(" ", Upper));

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
            string text = ReadText(encrypt);

            Console.WriteLine($"Prepared text : {text}");
            Console.WriteLine(encrypt
                ? $"Ciphertext    : {Transform(text, key, true)}"
                : $"Decrypted text: {Transform(text, key, false)}");
        }
    }
}