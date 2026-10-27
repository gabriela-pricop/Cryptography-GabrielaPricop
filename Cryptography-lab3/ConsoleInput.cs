namespace Cryptography_lab3;

public static class ConsoleInput
{
    public const int MinKeyLength = 7;
 
    private static string Prompt(string message)
    {
        Console.Write(message);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }
 
    public static int ReadChoice(string prompt, int min, int max)
    {
        while (true)
        {
            string input = Prompt(prompt);
 
            if (!int.TryParse(input, out int value))
            {
                Console.WriteLine($"Error: \"{input}\" is not a number. Enter a number from {min} to {max}.");
                continue;
            }
 
            if (value < min || value > max)
            {
                Console.WriteLine($"Error: {value} is not an option. Enter a number from {min} to {max}.");
                continue;
            }
 
            return value;
        }
    }
 
    public static string ReadKey()
    {
        while (true)
        {
            string input = Prompt($"Enter the key (letters A-Z or a-z, at least {MinKeyLength} characters): ");
 
            char bad = input.FirstOrDefault(c => !Alphabet.IsLatinLetter(c));
            if (bad != '\0')
            {
                string shown = bad == ' ' ? "space" : $"'{bad}'";
                Console.WriteLine($"Error: the character {shown} is not allowed in the key. " +
                                  "Use only letters from A to Z or from a to z, without spaces.");
                continue;
            }
 
            if (input.Length < MinKeyLength)
            {
                Console.WriteLine($"Error: the key has {input.Length} character(s). " +
                                  $"The minimum length is {MinKeyLength}.");
                continue;
            }
 
            return input.ToUpperInvariant();
        }
    }
 
    // Returns the text without spaces, in uppercase
    public static string ReadText(bool encrypt)
    {
        while (true)
        {
            Console.WriteLine();
            string input = Prompt(encrypt ? "Enter the message: " : "Enter the ciphertext: ");
 
            if (!Alphabet.TryPrepare(input, !encrypt, out string prepared, out char rejected))
            {
                string range = encrypt
                    ? "Use only letters from A to Z or from a to z."
                    : "Use only A-Z, a-z and the letters Ă, Â, Î, Ș, Ț produced by the cipher.";
                Console.WriteLine($"Error: the character '{rejected}' is not allowed. {range}");
                continue;
            }
 
            if (prepared.Length == 0)
            {
                Console.WriteLine("Error: the text is empty. Enter at least one letter.");
                continue;
            }
 
            return prepared;
        }
    }
}
 