using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync<TModel>(
        this ControllerContext controllerContext,
        string viewName,
        TModel model,
        ViewDataDictionary viewData,
        ITempDataDictionary tempData,
        bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName))
        {
            viewName = controllerContext.ActionDescriptor.ActionName;
        }

        viewData.Model = model;

        using (var writer = new StringWriter())
        {
            var viewEngine = controllerContext.HttpContext.RequestServices
                .GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                
            var viewResult = viewEngine.FindView(controllerContext, viewName, !partial);

            if (!viewResult.Success)
            {
                return $"A view with the name {viewName} could not be found";
            }

            var viewContext = new ViewContext(
                controllerContext,
                viewResult.View,
                viewData,
                tempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();
        }
    }
}