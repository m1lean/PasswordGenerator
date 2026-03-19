using Microsoft.AspNetCore.Mvc;
using PasswordGenerator.Models;
using PasswordGenerator.Services;

namespace PasswordGenerator.Controllers;

public class HomeController : Controller
{
    private readonly IPasswordService _pwd;
    private readonly IHistoryService _history;

    public HomeController(IPasswordService pwd, IHistoryService history)
    {
        _pwd = pwd;
        _history = history;
    }

    [HttpGet]
    public IActionResult Index()
        => View(new HomeViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateRequest request)
    {
        if (!ModelState.IsValid)
            return View("Index", new HomeViewModel { Request = request });

        List<GenerateResult> results;
        try
        {
            results = _pwd.Generate(request);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", new HomeViewModel { Request = request });
        }

        if (request.SaveToHistory)
            await _history.SaveAsync(request, results);

        var vm = new HomeViewModel
        {
            Request   = request,
            Results   = results,
            Generated = true
        };
        return View("Index", vm);
    }
}
