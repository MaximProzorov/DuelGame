using System.Net;

namespace Game.Models
{
    public class Response
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public static Response Ok() => new()
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccessStatusCode = true
        };

        public static Response NotFound() => new()
        {
            StatusCode = HttpStatusCode.NotFound,
            IsSuccessStatusCode = false
        };

        public static Response InternalServerError() => new()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccessStatusCode = false
        };

        public static Response InternalServerError(string errorMessage) => new()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccessStatusCode = false,
            ErrorMessage = errorMessage
        };

        public static Response BadRequest(string errorMessage) => new()
        {
            StatusCode = HttpStatusCode.BadRequest,
            IsSuccessStatusCode = false,
            ErrorMessage = errorMessage
        };
    }
    public class Response<T> : Response
    {
        public T Content { get; set; }

        public static Response<T> Ok(T content) => new()
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccessStatusCode = true,
            Content = content
        };

        public static new Response<T> InternalServerError() => new()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccessStatusCode = false
        };

        public static new Response<T> Unauthorized() => new()
        {
            StatusCode = HttpStatusCode.Unauthorized,
            IsSuccessStatusCode = false
        };

        public static new Response<T> InternalServerError(string errorMessage) => new()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccessStatusCode = false,
            ErrorMessage = errorMessage
        };

        public static new Response<T> BadRequest(string errorMessage) => new()
        {
            StatusCode = HttpStatusCode.BadRequest,
            IsSuccessStatusCode = false,
            ErrorMessage = errorMessage
        };

    }
}
