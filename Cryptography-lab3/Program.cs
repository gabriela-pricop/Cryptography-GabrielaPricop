using System.Text;
using Cryptography_lab3;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
 
        // Task 3.1 and Task 3.2
        ICipher[] ciphers = { new PlayfairCipher(), new VigenereCipher() };
 
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Polyalphabetic ciphers (Romanian alphabet, 31 letters) ===");
            Console.WriteLine("1 - Playfair (Task 3.1)");
            Console.WriteLine("2 - Vigenere (Task 3.2)");
            Console.WriteLine("0 - Exit");
            Console.WriteLine();
            int algorithm = ConsoleInput.ReadChoice("Choose the algorithm: ", 0, 2);
            if (algorithm == 0)
            {
                break;
            }
 
            ICipher cipher = ciphers[algorithm - 1];
            
            Console.WriteLine("1 - Encryption");
            Console.WriteLine("2 - Decryption");
            bool encrypt = ConsoleInput.ReadChoice("Choose the operation: ", 1, 2) == 1;
 
            string key = ConsoleInput.ReadKey();
 
            string? info = cipher.KeyInfo(key);
            if (info != null)
            {
                Console.WriteLine(info);
            }
 
            string text = ConsoleInput.ReadText(encrypt);
 
            try
            {
                string result = encrypt ? cipher.Encrypt(text, key) : cipher.Decrypt(text, key);
                Console.WriteLine();
                Console.WriteLine(encrypt ? $"Ciphertext: {result}" : $"Decrypted message: {result}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}