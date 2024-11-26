using Microsoft.AspNetCore.JsonPatch;
using SecretStore.Business.Interfaces;
using SecretStore.DataStore.Interface;
using SecretStore.Models;
using SecretStore.services;

namespace SecretStore.Business.Services;

public class PasswordEntriesBusinessService(ISecretStoreDataStore secretStoreDataStore, IPasswordEncryptionService encryptionService) : IPasswordEntriesBusinessService
{
    private readonly ISecretStoreDataStore _secretStoreDataStore = secretStoreDataStore ?? throw new ArgumentNullException(nameof(secretStoreDataStore));
    private readonly IPasswordEncryptionService _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(_encryptionService));

    public async Task<PasswordEntry?> CreatePasswordEntryAsync(PasswordEntry newPasswordEntry, CancellationToken cancellationToken = default)
    {
        if (newPasswordEntry == null)
        {
            throw new ArgumentNullException(nameof(newPasswordEntry), "Password entry cannot be null.");
        }

        newPasswordEntry.Password = _encryptionService.Encrypt(newPasswordEntry.Password);

        var entryId = await _secretStoreDataStore.CreatePasswordEntryAsync(newPasswordEntry, cancellationToken);

        var entry = await GetPasswordEntryByIdAsync(entryId, cancellationToken);

        return entry;
    }

    public async Task<bool> DeletePasswordEntryAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var deleted = await _secretStoreDataStore.DeletePasswordEntryAsync(entryId, cancellationToken);

        return deleted;
    }

    public async Task<List<PasswordEntry>> GetPasswordEntriesAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _secretStoreDataStore.GetPasswordEntriesAsync(cancellationToken);

        return entries;
    }

    public async Task<PasswordEntry?> GetPasswordEntryByIdAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _secretStoreDataStore.GetPasswordEntryByIdAsync(entryId, cancellationToken);

        entry.Password = _encryptionService.Decrypt(entry.Password);

        return entry;
    }

    public async Task<PasswordEntry?> UpdatePasswordEntryAsync(int entryId, JsonPatchDocument<PasswordEntry> entryPatch, CancellationToken cancellationToken = default)
    {
        var entry = await _secretStoreDataStore.GetPasswordEntryByIdAsync(entryId, cancellationToken);

        if (entry == null)
        {
            return entry;
        }

        entryPatch.ApplyTo(entry);

        entry.Password = _encryptionService.Encrypt(entry.Password);

        await _secretStoreDataStore.UpdatePasswordEntryAsync(entry, cancellationToken);

        entry = await _secretStoreDataStore.GetPasswordEntryByIdAsync(entryId, cancellationToken);

        entry.Password = _encryptionService.Decrypt(entry.Password);

        return entry;
    }
}
