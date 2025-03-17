using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BokioHarreWeb.Extensions;

public static class ControllerExtensions
{
    public static async Task<string> RenderViewToStringAsync<TModel>(
        this Controller controller,
        string viewName,
        TModel model
    )
    {
        var viewEngine = controller.HttpContext.RequestServices.GetService<IRazorViewEngine>();
        var viewResult = viewEngine!.GetView(
            viewPath: $"~/Views/{viewName}.cshtml", // Explicit path
            executingFilePath: $"~/Views/{viewName}.cshtml", 
            isMainPage: false
        );
        
        if (!viewResult.Success)
            throw new Exception($"View {viewName} not found");

        controller.ViewData.Model = model;

        await using var writer = new StringWriter();
        var viewContext = new ViewContext(
            controller.ControllerContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            writer,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}