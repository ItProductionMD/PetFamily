using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Responce;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.API.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToErrorActionResult(this UnitResult result)
        {
            if (result.Error is null)
                throw new InvalidOperationException("Result Errors is null or empty ");

            var statusCode = result.Error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Exception => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            var envelope = Envelope.Failure(result.Error);

            return new ObjectResult(envelope) { StatusCode = statusCode };
        }

        public static ActionResult ToErrorActionResult<T>(this Result<T> result)
        {
            if (result.Error is null)
                throw new InvalidOperationException("Result Error is null ");

            var statusCode = result.Error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Exception => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            var envelope = Envelope.Failure(result.Error, result.Data);

            return new ObjectResult(envelope) { StatusCode = statusCode };
        }

        public static Envelope ToEnvelope<T>(this Result<T> result)
        {
            var envelope = Envelope.Success(result.Data);
            return envelope;
        }

        public static Envelope ToEnvelope(this UnitResult result)
        {
            var envelope = Envelope.Success(null);
            return envelope;
        }
    }
}
