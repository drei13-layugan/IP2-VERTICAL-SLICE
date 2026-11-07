using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert success result to ProblemDetails.");
        }

        return CreateProblem(result.Error!);
    }

    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert success result to ProblemDetails.");
        }

        return CreateProblem(result.Error!);
    }

    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
        => result.IsSuccess ? onSuccess() : onFailure(result);

    private static IResult CreateProblem(Error error)
    {
        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = GetStatusCode(error.Code)
        };

        return Results.Problem(problemDetails);
    }

    private static int GetStatusCode(string errorCode) => errorCode switch
    {
        string code when code.Contains("NotFound") => 404,
        string code when code.Contains("Validation") => 400,
        string code when code.Contains("Conflict") || code.Contains("InUse") => 409,
        _ => 500
    };
}