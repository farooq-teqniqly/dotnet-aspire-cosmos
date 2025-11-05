namespace WineApi.Tests
{
    /// <summary>
    /// Provides route constants for testing WineApi endpoints.
    /// </summary>
    internal static class Routes
    {
        /// <summary>
        /// Routes for winery-related endpoints.
        /// </summary>
        internal static class Wineries
        {
            private const string Root = "wineries";

            /// <summary>
            /// Gets the route for creating a new winery.
            /// </summary>
            internal static string Create = Root;

            /// <summary>
            /// Gets the route for retrieving a winery by its ID.
            /// </summary>
            /// <param name="id">The unique identifier of the winery.</param>
            /// <returns>The route to get the winery.</returns>
            internal static string GetById(string id) => $"{Root}/{id}";
        }
    }
}
