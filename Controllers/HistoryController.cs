using Microsoft.AspNetCore.Mvc;
using PasswordGenerator.Services;
using PasswordGenerator.Models;

namespace PasswordGenerator.Controllers;

public class HistoryController : Controller
{
    private readonly IHistoryService _history;
    private readonly IExportService _export;
    private const int PageSize = 20;

    public HistoryController(IHistoryService history, IExportService export)
    {
        _history = history;
        _export  = export;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string sortBy = "date", int page = 1)
    {
        var (items, total) = await _history.GetPageAsync(search, sortBy, page, PageSize);
        var vm = new HistoryViewModel
        {
            Entries    = items,
            SearchTerm = search,
            SortBy     = sortBy,
            Page       = page,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)PageSize)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _history.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearAll()
    {
        await _history.ClearAllAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExportTxt()
    {
        var all = await _history.GetAllAsync();
        var bytes = _export.ExportTxt(all);
        return File(bytes, "text/plain", $"passwords_{DateTime.UtcNow:yyyyMMdd_HHmm}.txt");
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv()
    {
        var all = await _history.GetAllAsync();
        var bytes = _export.ExportCsv(all);
        return File(bytes, "text/csv", $"passwords_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
    }
}
