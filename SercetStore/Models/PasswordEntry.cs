namespace SecretStore.Models;

public class PasswordEntry
{
    public int Id { get; set; }
    public string? Website { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
