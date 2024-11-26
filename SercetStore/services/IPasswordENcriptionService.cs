namespace SecretStore.services;


public interface IPasswordEncryptionService
{
    string? Encrypt(string? plainText);
    string? Decrypt(string? cipherText);
}
