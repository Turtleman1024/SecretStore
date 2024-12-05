using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretStore.Business.Interfaces;
using SecretStore.Contracts.V1;
using SecretStore.Models;
using SecretStore.services;
using System.Threading;

namespace SecretStore.Controllers.V1;

[ApiController]
[Route("")]
public class PasswordEntriesController(IPasswordEntriesBusinessService businessService, ILogger<PasswordEntriesController> logger, IPasswordEncryptionService passwordEncryptionService) : ControllerBase
{
    private readonly IPasswordEncryptionService _passwordEncryptionService = passwordEncryptionService ?? throw new ArgumentNullException(nameof(passwordEncryptionService));
    private readonly IPasswordEntriesBusinessService _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));
    private readonly ILogger<PasswordEntriesController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Asynchronously get all User Passwords
    /// </summary>
    /// <returns>List of User Passwords</returns>
    [HttpGet, Route(ApiRoutes.Entries.GetPasswordEntries, Name = "GetPasswordEntriesAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPasswordEntriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _passwordEncryptionService.EnsureInitialized();

            var passwordEntries = await _businessService.GetPasswordEntriesAsync(cancellationToken);

            if ((passwordEntries?.Count ?? 0) == 0)
            {
                return NotFound("Could not find any Enrty");
            }

            return Ok(passwordEntries);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously get password entry by id
    /// </summary>
    /// <param name="entryId">The password entry id</param>
    /// <returns>The password entry</returns>
    [HttpGet, Route(ApiRoutes.Entries.GetPasswordEntryById, Name = "GetPasswordEntryByIdAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPasswordEntryByIdAsync([FromRoute] int entryId, CancellationToken cancellationToken = default)
    {
        try
        {
            _passwordEncryptionService.EnsureInitialized();

            var entry = await _businessService.GetPasswordEntryByIdAsync(entryId, cancellationToken);
            if (entry == null)
            {
                return NotFound($"Could not find Entry Id: {entryId}");
            }

            return Ok(entry);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously create a new entry
    /// </summary>
    /// <param name="newOwner">The new entry</param>
    /// <returns>The created entry </returns>
    [HttpPost, Route(ApiRoutes.Entries.CreatePasswordEntry, Name = "CreatePasswordEntryAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePasswordEntryAsync([FromBody] PasswordEntry entry, CancellationToken cancellationToken = default)
    {
        try
        {
            _passwordEncryptionService.EnsureInitialized();

            var result = await _businessService.CreatePasswordEntryAsync(entry, cancellationToken);

            if (result == null)
            {
                return BadRequest("Could not Create Entry");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously update entry information
    /// </summary>
    /// <param name="entryId">The current entry Id</param>
    /// <param name="entryPatch">The field to patch</param>
    /// <returns>The patched entry</returns>
    [HttpPatch, Route(ApiRoutes.Entries.UpdatePasswordEntry, Name = "UpdatePasswordEntryAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePasswordEntryAsync([FromRoute] int entryId, [FromBody] JsonPatchDocument<PasswordEntry> entryPatch, CancellationToken cancellationToken = default)
    {
        try
        {
            _passwordEncryptionService.EnsureInitialized();

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
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously delete an entry
    /// </summary>
    /// <param name="ownerId">The current entry id</param>
    [HttpDelete, Route(ApiRoutes.Entries.DeletePasswordEntry, Name = "DeletePasswordEntryAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePasswordEntryAsync([FromRoute] int entryId, CancellationToken cancellationToken = default)
    {
        try
        {
            _passwordEncryptionService.EnsureInitialized();

            var deleted = await _businessService.DeletePasswordEntryAsync(entryId, cancellationToken);

            if (deleted)
            {
                return Ok();
            }

            return NotFound("Entry has already been deleted or does not exist");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }

    }
}
