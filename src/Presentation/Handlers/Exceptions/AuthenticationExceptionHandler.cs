using System.Security.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.Presentation.Handlers.Exceptions
{
    public class AuthenticationExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<AuthenticationExceptionHandler> _logger;

        public AuthenticationExceptionHandler(ILogger<AuthenticationExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not AuthenticationException authenticationException)
            {
                return false;
            }

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = authenticationException.Message
            };

            _logger.LogError("Authentication failed: {AuthenticationExceptionMessage}", authenticationException.Message);

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
