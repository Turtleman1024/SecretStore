using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecretStore.services;

public class PasswordEncryptionService : IPasswordEncryptionService
{
    private byte[]? _key;
    private byte[]? _iv;

    public void Initialize(string encryptionKey)
    {
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
        _iv = _key.Take(16).ToArray();
    }

    public void EnsureInitialized()
    {
        if (_key == null || _iv == null)
        {
            throw new InvalidOperationException("Encryption service is not initialized. Load the encryption key first.");
        }
    }

    public string? Encrypt(string? plainText)
    {
        EnsureInitialized();

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
        EnsureInitialized();

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
