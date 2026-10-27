namespace Cryptography_lab3;

public interface ICipher
{
    string Name { get; }
 
    string Encrypt(string text, string key);
 
    string Decrypt(string text, string key);
 
    // Extra information about the key shown to the user (matrix, key codes), or null
    string? KeyInfo(string key);
}