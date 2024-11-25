using Microsoft.AspNetCore.JsonPatch;
using SecretStore.Business.Interfaces;
using SecretStore.DataStore.Interface;
using SecretStore.Models;

namespace SecretStore.Business.Services;

public class PasswordEntriesBusinessService(ISecretStoreDataStore secretStoreDataStore) : IPasswordEntriesBusinessService
{
    private readonly ISecretStoreDataStore _secretStoreDataStore = secretStoreDataStore ?? throw new ArgumentNullException(nameof(secretStoreDataStore));

    public async Task<PasswordEntry?> CreatePasswordEntryAsync(PasswordEntry newPasswordEntry, CancellationToken cancellationToken = default)
    {
        var entryId = await _secretStoreDataStore.CreatePasswordEntryAsync(newPasswordEntry, cancellationToken);

        var entry = await _secretStoreDataStore.GetPasswordEntryAsync(entryId, cancellationToken);

        return entry;
    }

    public async Task<bool> DeletePasswordEntryAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var deleted = await _secretStoreDataStore.DeletePasswordEntryAsync(entryId, cancellationToken);

        return deleted;
    }

    public async Task<List<PasswordEntry?>> GetPasswordEntriesAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _secretStoreDataStore.GetPasswordEntriesAsync(cancellationToken);

        return entries;
    }

    public async Task<PasswordEntry?> GetPasswordEntryByIdAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _secretStoreDataStore.GetPasswordEntryAsync(entryId, cancellationToken);

        return entry;
    }

    public async Task<PasswordEntry?> UpdatePasswordEntryAsync(int entryId, JsonPatchDocument<PasswordEntry> entryPatch, CancellationToken cancellationToken = default)
    {
        var entry = await _secretStoreDataStore.GetPasswordEntryAsync(entryId, cancellationToken);

        if (entry == null)
        {
            return entry;
        }

        entryPatch.ApplyTo(entry);

        await _secretStoreDataStore.UpdatePasswordEntryAsync(entry, cancellationToken);

        entry = await _secretStoreDataStore.GetPasswordEntryAsync(entryId, cancellationToken);

        return entry;
    }
}
