using System.Diagnostics;
using BokioHarreWeb.Extensions;
using BokioHarreWeb.Hubs;
using Microsoft.AspNetCore.Mvc;
using BokioHarreWeb.Models;
using Common.Services;
using Microsoft.AspNetCore.SignalR;

namespace BokioHarreWeb.Controllers;

public class JournalEntriesController(IJournalService journalService, IHubContext<ItemHub> hubContext) : Controller
{

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> StartStreaming(int pageIndex = 0, int pageSize = 1000)
    {
        var journalEntries = journalService.GetJournalEntries();

        var itemCount = 0;
        var currentItemCount = 0;
        await foreach (var item in journalEntries)
        {
            itemCount++;

            if (itemCount <= pageIndex * pageSize)
            {
                continue;
            }
            
            currentItemCount++;
            if (currentItemCount > pageSize)
            {
                await hubContext.Clients.All.SendAsync("UpdateLoadMore", true);
                return NoContent();
            }
            
            // Render the _Item.cshtml partial view to HTML
            var html = await this.RenderViewToStringAsync("JournalEntries/_Item", item);
            
            // Send each item directly via the hub context
            await hubContext.Clients.All.SendAsync("ReceiveItemHtml", html);
        }
    
        await hubContext.Clients.All.SendAsync("UpdateLoadMore", false);
        return NoContent();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}