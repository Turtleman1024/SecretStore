using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecretStore.services;

public class PasswordEncryptionService : IPasswordEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public PasswordEncryptionService(string encryptionKey)
    {
        // Derive a 32-byte key and a 16-byte IV from the provided encryptionKey
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
        _iv = _key.Take(16).ToArray(); // Use the first 16 bytes as IV
    }

    public string? Encrypt(string? plainText)
    {
        // Check if the input string is null, return an empty string if it is.
        if (plainText == null)
        {
            return null;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string? Decrypt(string? cipherText)
    {
        // Check if the input string is null, return an empty string if it is.
        if (cipherText == null)
        {
            return null;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        {
            return reader.ReadToEnd();
        }
    }
}
