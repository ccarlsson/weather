using System.Net;

namespace Weather.Services;

public enum WeatherServiceErrorKind
{
    BadRequest,
    Unauthorized,
    NotFound,
    RateLimited,
    UpstreamFailure,
    InvalidResponse
}

public sealed class WeatherServiceException : Exception
{
    public WeatherServiceException(
        WeatherServiceErrorKind errorKind,
        string message,
        HttpStatusCode? statusCode = null,
        string? responseBody = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorKind = errorKind;
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public WeatherServiceErrorKind ErrorKind { get; }

    public HttpStatusCode? StatusCode { get; }

    public string? ResponseBody { get; }
}
