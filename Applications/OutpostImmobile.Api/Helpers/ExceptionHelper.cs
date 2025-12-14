using System.Net;
using OutpostImmobile.Api.Response;

namespace OutpostImmobile.Api.Helpers;

public class ExceptionHelper
{
    public static TypedResponse<T> HandleErrors<T>(T data, string message)
    {
        return new TypedResponse<T>
        {
            StatusCode = HttpStatusCode.BadRequest,
            Data = data,
            Errors = message
        };
    }
}