using FluentResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GtKanu.Infrastructure.Extensions;

public static class FluentResultsExtensions
{
    public static void ToModelState(this IReadOnlyList<IError> errors, ModelStateDictionary modelState)
    {
        foreach(var e in errors)
        {
            modelState.AddModelError(string.Empty, e.Message);
        }
    }
}
