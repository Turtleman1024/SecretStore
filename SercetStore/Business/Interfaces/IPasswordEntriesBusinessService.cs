using Microsoft.AspNetCore.JsonPatch;
using SecretStore.Models;

namespace SecretStore.Business.Interfaces;

public interface IPasswordEntriesBusinessService
{
    Task<List<PasswordEntry>> GetPasswordEntriesAsync();

    Task<PasswordEntry> GetPasswordEntryByIdAsync(int entryId);

    Task<PasswordEntry> CreatePasswordEntryAsync(PasswordEntry newPasswordEntry);

    Task<PasswordEntry> UpdatePasswordEntryAsync(int entryId, JsonPatchDocument<PasswordEntry> entryPatch);

    Task<bool> DeletePasswordEntryAsync(int entryId);
}
