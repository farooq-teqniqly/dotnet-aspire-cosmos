using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WineApi.Middleware
{
    /// <summary>
    /// Handles unhandled exceptions globally and returns a standardized problem details response.
    /// </summary>
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        /// <param name="problemDetailsService">The service for writing problem details responses.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="problemDetailsService"/> is null.</exception>
        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            ArgumentNullException.ThrowIfNull(problemDetailsService);

            _problemDetailsService = problemDetailsService;
        }

        /// <summary>
        /// Attempts to handle the specified exception by writing a standardized problem details response.
        /// </summary>
        /// <param name="httpContext">The HTTP context for the current request.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the exception was handled.</returns>
        public ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            ArgumentNullException.ThrowIfNull(exception);

            return _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                        Title = "Internal Server Error",
                        Detail =
                            "An error occurred while processing your request. Please try again",
                    },
                }
            );
        }
    }
}
