namespace GtKanu.Infrastructure.Tests.Extensions;

using FluentResults;
using GtKanu.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class FluentResultsExtensionsTests
{
    [Fact]
    public void ToModelState_ShouldAddErrors()
    {
        var errors = new List<IError>
        {
            Result.Fail("Error 1").Errors[0],
            Result.Fail("Error 2").Errors[0]
        };

        var modelState = new ModelStateDictionary();
        errors.ToModelState(modelState);

        modelState.Should().ContainSingle();
        modelState[string.Empty]!.Errors.Should().HaveCount(2);
        modelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "Error 1");
        modelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "Error 2");
    }

    [Fact]
    public void ToModelState_EmptyErrors_ShouldNotAddAnything()
    {
        var errors = new List<IError>();
        var modelState = new ModelStateDictionary();
        errors.ToModelState(modelState);
        modelState.Should().BeEmpty();
    }
}
