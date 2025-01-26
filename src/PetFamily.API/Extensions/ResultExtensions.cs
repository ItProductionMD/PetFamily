using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Responce;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.API.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToErrorActionResult(this Result result)
        {
            if (result.Errors is null || result.Errors.Count==0)
                throw new InvalidOperationException("Result Errors is null or empty ");

            var statusCode = result.Errors.FirstOrDefault()!.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Exception => StatusCodes.Status500InternalServerError,
                _=>StatusCodes.Status500InternalServerError
            };

            var envelope = Envelope.Failure(result.Errors);

            return new ObjectResult(envelope) { StatusCode= statusCode };
        }

        public static ActionResult ToErrorActionResult<T>(this Result<T> result)
        {
            if (result.Errors is null)
                throw new InvalidOperationException("Result Error is null ");

            var statusCode = result.Errors.FirstOrDefault()!.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Exception => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            var envelope = Envelope.Failure(result.Errors);

            return new ObjectResult(envelope) { StatusCode = statusCode };
        }

        public static Envelope ToEnvelope<T>(this Result<T> result)
        {
            var envelope = Envelope.Success(result.Data);
            return envelope;
        }

        public static Envelope ToEnvelope(this Result result)
        {
            var envelope = Envelope.Success(null);
            return envelope;
        }
    }
}
