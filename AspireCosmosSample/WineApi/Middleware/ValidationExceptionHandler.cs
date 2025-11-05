using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WineApi.Middleware
{
    /// <summary>
    /// Handles FluentValidation exceptions and returns a standardized problem details response with validation errors.
    /// </summary>
    internal sealed class ValidationExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationExceptionHandler"/> class.
        /// </summary>
        /// <param name="problemDetailsService">The service for writing problem details responses.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="problemDetailsService"/> is null.</exception>
        public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            ArgumentNullException.ThrowIfNull(problemDetailsService);

            _problemDetailsService = problemDetailsService;
        }

        /// <summary>
        /// Attempts to handle validation exceptions by writing a problem details response with validation error details.
        /// </summary>
        /// <param name="httpContext">The HTTP context for the current request.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if the exception was handled; otherwise, false.</returns>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            ArgumentNullException.ThrowIfNull(exception);

            if (exception is not ValidationException validationException)
            {
                return false;
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var context = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Detail = "One or more validation errors occurred",
                    Status = StatusCodes.Status400BadRequest,
                },
            };

            var errors = validationException
                .Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key.ToLowerInvariant(),
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            context.ProblemDetails.Extensions.Add("errors", errors);

            return await _problemDetailsService.TryWriteAsync(context).ConfigureAwait(false);
        }
    }
}
