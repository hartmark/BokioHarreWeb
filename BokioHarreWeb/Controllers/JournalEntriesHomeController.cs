using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BokioHarreWeb.Apis;
using BokioHarreWeb.Models;

namespace BokioHarreWeb.Controllers;

public class JournalEntriesController(ILogger<JournalEntriesController> logger, IBokioApi bokioApi) : Controller
{

    public async Task<IActionResult> Index(int page = 1)
    {
        var response = await bokioApi.GetJournalEntries(
            page);
        if (!response.IsSuccessful)
        {
            throw response.Error;
        }
        
        logger.LogDebug("Got {JournalEntries}", response.Content);
        
        return View(response.Content);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}