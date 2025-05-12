using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ProductInventoryManagementSystem.Services
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError; //Default = 500
            var title = "Internal Server Error";
            var detail = exception.Message;

            switch (exception)
            {
                case AccessViolationException:
                    statusCode = (int)HttpStatusCode.Forbidden; // 403
                    title = "Unauthorized";
                    break;
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized; //401
                    title = "Unauthenticated";
                    break;
                case BadHttpRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest; //400
                    title = "Bad Request";
                    break;
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound; //404
                    title = "NotFound";
                    break;
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails()
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
            return true;



        }
    }
}
