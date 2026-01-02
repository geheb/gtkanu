using FluentResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace GtKanu.Core.Extensions;

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
