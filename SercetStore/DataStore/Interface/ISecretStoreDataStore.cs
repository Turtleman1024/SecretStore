using SecretStore.Models;

namespace SecretStore.DataStore.Interface;

public interface ISecretStoreDataStore
{
    Task<List<PasswordEntry>> GetPasswordEntriesAsync(CancellationToken cancellationToken = default);

    Task<PasswordEntry> GetPasswordEntryByIdAsync(int entryId, CancellationToken cancellationToken = default);

    Task<int> CreatePasswordEntryAsync(PasswordEntry entry, CancellationToken cancellationToken = default);

    Task UpdatePasswordEntryAsync(PasswordEntry entry, CancellationToken cancellationToken = default);

    Task<bool> DeletePasswordEntryAsync(int entryId, CancellationToken cancellationToken = default);
}
