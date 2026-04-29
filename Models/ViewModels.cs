namespace PasswordGenerator.Models;

public class GenerateResult
{
    public string Value { get; set; } = string.Empty;
    public double Entropy { get; set; }
    public string StrengthLabel { get; set; } = string.Empty;
    public int StrengthLevel { get; set; }  // 0-4
}

public class HomeViewModel
{
    public GenerateRequest Request { get; set; } = new();
    public List<GenerateResult> Results { get; set; } = new();
    public bool Generated { get; set; }
}

public class HistoryViewModel
{
    public List<PasswordEntry> Entries { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "date";
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}