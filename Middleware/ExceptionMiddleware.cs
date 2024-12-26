// using Microsoft.AspNetCore.Http;
// using System.Net;
// using System.Text.Json;

// namespace ClassLibrary.Middleware
// {

// public class ExceptionMiddleware
// {
//     private readonly RequestDelegate _next;

//     public ExceptionMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }

//     public async Task InvokeAsync(HttpContext context)
//     {
//         try
//         {
//             await _next(context);
//         }
//         catch (Exception ex)
//         {
//             await HandleExceptionAsync(context, ex);
//         }
//     }

//     private Task HandleExceptionAsync(HttpContext context, Exception exception)
//     {
//         var statusCode = exception switch
//         {
//             ValidationException => HttpStatusCode.BadRequest,
//             DatabaseException => HttpStatusCode.InternalServerError,
//             _ => HttpStatusCode.InternalServerError
//         };

//         var response = new
//         {
//             message = exception.Message,
//             errorType = exception.GetType().Name,
//         };

//         context.Response.ContentType = "application/json";
//         context.Response.StatusCode = (int)statusCode;

//         return context.Response.WriteAsync(JsonSerializer.Serialize(response));
//     }
// }

// public class AppException : Exception
// {
//     public AppException(string message) : base(message) { }
// }

// public class ValidationException : AppException
// {
//     public ValidationException(string message) : base(message) { }
// }

// public class DatabaseException : AppException
// {
//     public DatabaseException(string message, Exception innerException = null)
//         : base(message, innerException) { }
// }


// }