using Microsoft.AspNetCore.Mvc;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Extensions
{
    public static class ErrorExtensions
    {
        public static ActionResult ToActionResult(this Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Exception => StatusCodes.Status500InternalServerError,
                _=>StatusCodes.Status500InternalServerError
            };
             return new ObjectResult(error) { StatusCode= statusCode };
        }
    }
}
