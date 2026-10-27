using System.Text;

namespace Cryptography_lab3;

public class VigenereCipher : ICipher
{
    public string Name => "Vigenere";
 
    // c = (m + k) mod 31
    public string Encrypt(string text, string key)
    {
        return Transform(text, key, 1);
    }
 
    // m = (c - k) mod 31
    public string Decrypt(string text, string key)
    {
        return Transform(text, key, -1);
    }
 
    public string? KeyInfo(string key)
    {
        Console.WriteLine();
        var letters = new StringBuilder("Key letters: ");
        var codes = new StringBuilder("Key codes  : ");
 
        foreach (char c in key)
        {
            letters.Append(c.ToString().PadLeft(3));
            codes.Append(Alphabet.CodeOf(c).ToString().PadLeft(3));
        }
 
        return letters + Environment.NewLine + codes;
    }
 
    private static string Transform(string text, string key, int sign)
    {
        var sb = new StringBuilder(text.Length);
 
        for (int i = 0; i < text.Length; i++)
        {
            int m = Alphabet.CodeOf(text[i]);
            int k = Alphabet.CodeOf(key[i % key.Length]); // key is repeated
            sb.Append(Alphabet.Letters[Alphabet.Mod(m + sign * k, Alphabet.Size)]);
        }
 
        return sb.ToString();
    }
}
