namespace SecretStore.services;


public interface IPasswordEncryptionService
{
    void Initialize(string encryptionKey);
    void EnsureInitialized();
    string? Encrypt(string? plainText);
    string? Decrypt(string? cipherText);
}
