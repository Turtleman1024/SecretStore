using Microsoft.AspNetCore.Mvc;
using SecretStore.Business.Interfaces;
using SecretStore.Contracts.V1;
using SecretStore.services;

namespace SecretStore.Controllers.V1;

[ApiController]
[Route("")]
public class LoginController(IPasswordEncryptionService passwordEncryptionService) : ControllerBase
{
    private readonly IPasswordEncryptionService _passwordEncryptionService = passwordEncryptionService  ?? throw new ArgumentNullException(nameof(passwordEncryptionService));

    [HttpPost, Route(ApiRoutes.Login.LoadEncryptionKey, Name = "LoadEncryptionKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult LoadEncryptionKey([FromBody] string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(encryptionKey))
        {
            return BadRequest("Encryption key cannot be null or empty.");
        }

        try
        {
            _passwordEncryptionService.Initialize(encryptionKey);
            return Ok("Encryption key loaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error initializing encryption key: {ex.Message}");
        }
    }
}
