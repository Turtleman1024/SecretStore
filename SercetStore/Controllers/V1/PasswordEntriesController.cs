using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretStore.Business.Interfaces;
using SecretStore.Contracts.V1;
using SecretStore.Models;
using System.Threading;

namespace SecretStore.Controllers.V1;

[ApiController]
[Route("")]
public class PasswordEntriesController(IPasswordEntriesBusinessService businessService, ILogger<PasswordEntriesController> logger) : ControllerBase
{
    private readonly IPasswordEntriesBusinessService _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));
    private readonly ILogger<PasswordEntriesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Asynchronously get all User Passwords
    /// </summary>
    /// <returns>List of User Passwords</returns>
    [HttpGet, Route(ApiRoutes.Entries.GetPasswordEntries, Name = "GetPasswordEntriesAsync")]
    public async Task<IActionResult> GetPasswordEntriesAsync(CancellationToken cancellationToken = default)
    {
        var passwordEntries = await _businessService.GetPasswordEntriesAsync(cancellationToken);

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
    [HttpGet, Route(ApiRoutes.Entries.GetPasswordEntryById, Name = "GetPasswordEntryByIdAsync")]
    public async Task<IActionResult> GetPasswordEntryByIdAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var entry = await _businessService.GetPasswordEntryByIdAsync(entryId, cancellationToken);
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
    public async Task<IActionResult> CreatePasswordEntryAsync([FromBody] PasswordEntry entry, CancellationToken cancellationToken = default)
    {
        var result = await _businessService.CreatePasswordEntryAsync(entry, cancellationToken);

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
    /// <param name="entryPatch">The field to patch</param>
    /// <returns>The patched entry</returns>
    [HttpPatch, Route(ApiRoutes.Entries.UpdatePasswordEntry, Name = "UpdatePasswordEntryAsync")]
    public async Task<IActionResult> UpdatePasswordEntryAsync(int entryId, [FromBody] JsonPatchDocument<PasswordEntry> entryPatch, CancellationToken cancellationToken = default)
    {
        if (entryPatch == null || entryPatch.Operations.Count == 0)
        {
            return BadRequest(new { Message = "Invalid patch document." });
        }

        var updatedEntry = await _businessService.UpdatePasswordEntryAsync(entryId, entryPatch, cancellationToken);

        if (updatedEntry == null)
        {
            return NotFound(new { Message = $"Password entry with ID {entryId} not found." });
        }

        return Ok(updatedEntry);
    }

    /// <summary>
    /// Asynchronously delete an entry
    /// </summary>
    /// <param name="ownerId">The current entry id</param>
    [HttpDelete, Route(ApiRoutes.Entries.DeletePasswordEntry, Name = "DeletePasswordEntryAsync")]
    public async Task<IActionResult> DeletePasswordEntryAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var deleted = await _businessService.DeletePasswordEntryAsync(entryId);

        if (deleted)
        {
            return Ok();
        }

        return NotFound("Entry has already been deleted or does not exist");

    }
}
