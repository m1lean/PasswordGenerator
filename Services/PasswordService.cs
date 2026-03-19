using System.Security.Cryptography;
using PasswordGenerator.Models;

namespace PasswordGenerator.Services;

public interface IPasswordService
{
    List<GenerateResult> Generate(GenerateRequest request);
    string BuildCharset(GenerateRequest request);
    double CalcEntropy(int length, int charsetSize);
    (string Label, int Level) GetStrength(double entropy);
}

public class PasswordService : IPasswordService
{
    private const string Upper   = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower   = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits  = "0123456789";
    private const string Symbols = "!@#$%^&*_-+=;:,.?/";
    private const string Brackets = "()[]{}";
    private const string Ambiguous = "0Ol1I";

    public List<GenerateResult> Generate(GenerateRequest req)
    {
        var charset = BuildCharset(req);
        if (string.IsNullOrEmpty(charset))
            throw new InvalidOperationException("Выберите хотя бы один набор символов.");

        var results = new List<GenerateResult>();
        for (int i = 0; i < req.Count; i++)
        {
            string pwd = req.NoRepeat
                ? GenerateNoRepeat(req.Length, charset)
                : GenerateStandard(req.Length, charset);

            double entropy = CalcEntropy(pwd.Length, charset.Length);
            var (label, level) = GetStrength(entropy);

            results.Add(new GenerateResult
            {
                Value = pwd,
                Entropy = Math.Round(entropy, 1),
                StrengthLabel = label,
                StrengthLevel = level
            });
        }
        return results;
    }

    public string BuildCharset(GenerateRequest req)
    {
        var chars = new System.Text.StringBuilder();
        if (req.UseUpper)   chars.Append(Upper);
        if (req.UseLower)   chars.Append(Lower);
        if (req.UseDigits)  chars.Append(Digits);
        if (req.UseSymbols) chars.Append(Symbols);
        if (req.UseBrackets) chars.Append(Brackets);
        if (req.UseSpace)   chars.Append(' ');
        if (!string.IsNullOrEmpty(req.CustomChars)) chars.Append(req.CustomChars);

        var set = chars.ToString().Distinct();
        if (req.ExcludeAmbiguous)
            set = set.Where(c => !Ambiguous.Contains(c));

        return new string(set.ToArray());
    }

    public double CalcEntropy(int length, int charsetSize)
        => charsetSize <= 1 ? 0 : length * Math.Log2(charsetSize);

    public (string Label, int Level) GetStrength(double entropy) => entropy switch
    {
        < 28  => ("Очень слабый", 0),
        < 36  => ("Слабый",       1),
        < 60  => ("Средний",      2),
        < 128 => ("Сильный",      3),
        _     => ("Очень сильный",4)
    };

    // --- private helpers ---

    private static string GenerateStandard(int length, string charset)
    {
        var buf = new char[length];
        for (int i = 0; i < length; i++)
            buf[i] = charset[SecureIndex(charset.Length)];
        return new string(buf);
    }

    private static string GenerateNoRepeat(int length, string charset)
    {
        var pool = charset.ToList();
        length = Math.Min(length, pool.Count);
        var buf = new char[length];
        for (int i = 0; i < length; i++)
        {
            int idx = SecureIndex(pool.Count);
            buf[i] = pool[idx];
            pool.RemoveAt(idx);
        }
        return new string(buf);
    }

    private static int SecureIndex(int max)
    {
        // Rejection sampling — no modulo bias
        uint limit = uint.MaxValue - (uint.MaxValue % (uint)max);
        uint val;
        do { val = BitConverter.ToUInt32(RandomNumberGenerator.GetBytes(4)); }
        while (val >= limit);
        return (int)(val % (uint)max);
    }
}
