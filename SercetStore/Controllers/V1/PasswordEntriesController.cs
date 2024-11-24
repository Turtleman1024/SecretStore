using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SecretStore.Business.Interfaces;
using SecretStore.Contracts.V1;
using SecretStore.Models;

namespace SecretStore.Controllers.V1;

[ApiController]
[Route("[controller]")]
public class PasswordEntriesController : ControllerBase
{
    private readonly IPasswordEntriesBusinessService _businessService;

    public PasswordEntriesController(IPasswordEntriesBusinessService businessService)
    {
        _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));
    }

    /// <summary>
    /// Asynchronously get all User Passwords
    /// </summary>
    /// <returns>List of User Passwords</returns>
    [HttpGet, Route(ApiRoutes.Entries.GetPasswordEntries, Name = "GetPasswordEntriesAsync")]
    public async Task<IActionResult> GetPasswordEntriesAsync()
    {
        var passwordEntries = await _businessService.GetPasswordEntriesAsync();

        if ((passwordEntries?.Count ?? 0) == 0)
        {
            return NotFound("Could not find any Enrty");
        }

        return Ok(passwordEntries);
    }

    /// <summary>
    /// Asynchronously get password entry by id
    /// </summary>
    /// <param name="entryId">The password entry id</param>
    /// <returns>The password entry</returns>
    [HttpGet, Route(ApiRoutes.Entries.GetPasswordEntryById, Name = "GetPasswordEntryByIdAync")]
    public async Task<IActionResult> GetPasswordEntryByIdAync(int entryId)
    {
        var entry = await _businessService.GetPasswordEntryByIdAsync(entryId);
        if (entry == null)
        {
            return NotFound($"Could not find Entry Id: {entryId}");
        }

        return Ok(entry);
    }

    /// <summary>
    /// Asynchronously create a new entry
    /// </summary>
    /// <param name="newOwner">The new entry</param>
    /// <returns>The created entry </returns>
    [HttpPost, Route(ApiRoutes.Entries.CreatePasswordEntry, Name = "CreatePasswordEntryAsync")]
    public async Task<IActionResult> CreatePasswordEntryAsync([FromBody] PasswordEntry entry)
    {
        var result = await _businessService.CreatePasswordEntryAsync(entry);

        if (result == null)
        {
            return BadRequest("Could not Create Entry");
        }

        return Ok(result);
    }

    /// <summary>
    /// Asynchronously update entry information
    /// </summary>
    /// <param name="entryId">The current entry Id</param>
    /// <param name="ownerPatch">The field to patch</param>
    /// <returns>The patched entry</returns>
    [HttpPatch, Route(ApiRoutes.Entries.UpdatePasswordEntry, Name = "UpdatePasswordEntryAsync")]
    public async Task<IActionResult> UpdateOwnerAsync(int entryId, [FromBody] JsonPatchDocument<PasswordEntry> ownerPatch)
    {
        if (ownerPatch?.Operations?.Count > 0)
        {
            var owner = await _businessService.UpdatePasswordEntryAsync(entryId, ownerPatch);
            if (owner != null)
            {
                return Ok(owner);
            }
            return BadRequest($"Could not Patch Entry with Id: {entryId}");
        }

        return BadRequest($"Could not Patch Entry with Id: {entryId}");
    }

    /// <summary>
    /// Asynchronously delete an entry
    /// </summary>
    /// <param name="ownerId">The current entry id</param>
    [HttpDelete, Route(ApiRoutes.Entries.DeletePasswordEntry, Name = "DeletePasswordEntryAsync")]
    public async Task<IActionResult> DeletePasswordEntryAsync(int entryId)
    {
        var deleted = await _businessService.DeletePasswordEntryAsync(entryId);

        if (deleted)
        {
            return Ok();
        }

        return NotFound("Entry has already been deleted or does not exist");

    }
}
