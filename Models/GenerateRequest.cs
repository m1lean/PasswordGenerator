using System.ComponentModel.DataAnnotations;

namespace PasswordGenerator.Models;

public class GenerateRequest
{
    [Range(4, 128)]
    public int Length { get; set; } = 16;

    [Range(1, 100)]
    public int Count { get; set; } = 5;

    public bool UseUpper { get; set; } = true;
    public bool UseLower { get; set; } = true;
    public bool UseDigits { get; set; } = true;
    public bool UseSymbols { get; set; } = true;
    public bool UseBrackets { get; set; }
    public bool UseSpace { get; set; }
    public bool ExcludeAmbiguous { get; set; }
    public bool NoRepeat { get; set; }
    public string? CustomChars { get; set; }
    public bool SaveToHistory { get; set; } = true;
}
