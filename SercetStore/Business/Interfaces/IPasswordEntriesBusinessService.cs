using Microsoft.AspNetCore.JsonPatch;
using SecretStore.Models;

namespace SecretStore.Business.Interfaces;

public interface IPasswordEntriesBusinessService
{
    Task<List<PasswordEntry?>> GetPasswordEntriesAsync(CancellationToken cancellationToken = default);

    Task<PasswordEntry?> GetPasswordEntryByIdAsync(int entryId, CancellationToken cancellationToken = default);

    Task<PasswordEntry?> CreatePasswordEntryAsync(PasswordEntry newPasswordEntry, CancellationToken cancellationToken = default);

    Task<PasswordEntry?> UpdatePasswordEntryAsync(int entryId, JsonPatchDocument<PasswordEntry> entryPatch, CancellationToken cancellationToken = default);

    Task<bool> DeletePasswordEntryAsync(int entryId, CancellationToken cancellationToken = default);
}
