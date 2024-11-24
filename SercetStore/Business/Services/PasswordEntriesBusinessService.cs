using Microsoft.AspNetCore.JsonPatch;
using SecretStore.Business.Interfaces;
using SecretStore.DataStore.Interface;
using SecretStore.Models;

namespace SecretStore.Business.Services;

public class PasswordEntriesBusinessService : IPasswordEntriesBusinessService
{
    private readonly ISecretStoreDataStore _secretStoreDataStore;

    public PasswordEntriesBusinessService(ISecretStoreDataStore secretStoreDataStore)
    {
        _secretStoreDataStore = secretStoreDataStore ?? throw new ArgumentNullException(nameof(secretStoreDataStore));
    }

    public async Task<PasswordEntry> CreatePasswordEntryAsync(PasswordEntry newPasswordEntry)
    {
        var entryId = await _secretStoreDataStore.CreatePasswordEntryAsync(newPasswordEntry);

        var entry = await _secretStoreDataStore.GetPasswordEntryAsync(entryId);

        return entry;
    }

    public async Task<bool> DeletePasswordEntryAsync(int entryId)
    {
        var deleted = await _secretStoreDataStore.DeletePasswordEntryAsync(entryId);

        return deleted;
    }

    public async Task<List<PasswordEntry>> GetPasswordEntriesAsync()
    {
        var entries = await _secretStoreDataStore.GetPasswordEntriesAsync();

        return entries;
    }

    public async Task<PasswordEntry> GetPasswordEntryByIdAsync(int entryId)
    {
        var entry = await _secretStoreDataStore.GetPasswordEntryAsync(entryId);

        return entry;
    }

    public Task<PasswordEntry> UpdatePasswordEntryAsync(int entryId, JsonPatchDocument<PasswordEntry> entryPatch)
    {
        throw new NotImplementedException();
    }
}
