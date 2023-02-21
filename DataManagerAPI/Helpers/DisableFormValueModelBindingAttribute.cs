using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DataManagerAPI.Helpers;

/// <summary>
/// DisableFormValueModelBindingAttribute is used to disable model binding
/// https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0#upload-large-files-with-streaming
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
{
    /// <summary>
    /// Removes all providers from IValueProviderFactory.
    /// </summary>
    /// <param name="context"><see cref="ResourceExecutingContext"/></param>
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var factories = context.ValueProviderFactories;
        factories.RemoveType<FormValueProviderFactory>();
        factories.RemoveType<FormFileValueProviderFactory>();
        factories.RemoveType<JQueryFormValueProviderFactory>();
    }

    /// <summary>
    /// Nothing to do.
    /// </summary>
    /// <param name="context"><see cref="ResourceExecutedContext"/></param>
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}