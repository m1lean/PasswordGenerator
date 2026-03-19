using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Data;
using PasswordGenerator.Models;

namespace PasswordGenerator.Services;

public interface IHistoryService
{
    Task SaveAsync(GenerateRequest req, List<GenerateResult> results);
    Task<(List<PasswordEntry> Items, int Total)> GetPageAsync(string? search, string sortBy, int page, int pageSize = 20);
    Task DeleteAsync(int id);
    Task ClearAllAsync();
    Task<List<PasswordEntry>> GetAllAsync();
}

public class HistoryService : IHistoryService
{
    private readonly AppDbContext _db;
    private readonly IPasswordService _svc;

    public HistoryService(AppDbContext db, IPasswordService svc)
    {
        _db = db;
        _svc = svc;
    }

    public async Task SaveAsync(GenerateRequest req, List<GenerateResult> results)
    {
        var charSets = string.Join(", ", new[]
        {
            req.UseUpper   ? "A-Z" : null,
            req.UseLower   ? "a-z" : null,
            req.UseDigits  ? "0-9" : null,
            req.UseSymbols ? "!@#" : null,
            req.UseBrackets ? "()[]{}" : null,
            req.UseSpace   ? "space" : null,
        }.Where(x => x != null));

        var entries = results.Select(r => new PasswordEntry
        {
            Value        = r.Value,
            Length       = r.Value.Length,
            CharSets     = charSets,
            Entropy      = r.Entropy,
            StrengthLabel = r.StrengthLabel,
            CreatedAt    = DateTime.UtcNow
        });

        _db.PasswordEntries.AddRange(entries);
        await _db.SaveChangesAsync();
    }

    public async Task<(List<PasswordEntry> Items, int Total)> GetPageAsync(
        string? search, string sortBy, int page, int pageSize = 20)
    {
        var q = _db.PasswordEntries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(e => e.StrengthLabel.Contains(search) ||
                              e.CharSets.Contains(search) ||
                              e.Note != null && e.Note.Contains(search));

        q = sortBy switch
        {
            "length"   => q.OrderByDescending(e => e.Length),
            "strength" => q.OrderByDescending(e => e.Entropy),
            _          => q.OrderByDescending(e => e.CreatedAt)
        };

        int total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _db.PasswordEntries.FindAsync(id);
        if (e != null) { _db.PasswordEntries.Remove(e); await _db.SaveChangesAsync(); }
    }

    public async Task ClearAllAsync()
    {
        _db.PasswordEntries.RemoveRange(_db.PasswordEntries);
        await _db.SaveChangesAsync();
    }

    public Task<List<PasswordEntry>> GetAllAsync()
        => _db.PasswordEntries.OrderByDescending(e => e.CreatedAt).ToListAsync();
}
