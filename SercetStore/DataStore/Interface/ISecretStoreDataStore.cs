using SecretStore.Models;

namespace SecretStore.DataStore.Interface;

public interface ISecretStoreDataStore
{
    Task<List<PasswordEntry>> GetPasswordEntriesAsync();

    Task<PasswordEntry> GetPasswordEntryAsync(int entryId);

    Task<int> CreatePasswordEntryAsync(PasswordEntry entry);

    Task<PasswordEntry> UpdatePasswordEntryAsync(PasswordEntry entry);

    Task<bool> DeletePasswordEntryAsync(int entryId);
}
