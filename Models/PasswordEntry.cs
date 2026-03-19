namespace PasswordGenerator.Models;

public class PasswordEntry
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Length { get; set; }
    public string CharSets { get; set; } = string.Empty;  // comma-separated flags
    public double Entropy { get; set; }
    public string StrengthLabel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
}
