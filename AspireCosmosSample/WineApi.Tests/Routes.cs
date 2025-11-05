namespace WineApi.Tests
{
    internal static class Routes
    {
        internal static class Wineries
        {
            private static string Root = "wineries";
            internal static string Create = Root;

            internal static string GetById(string id) => $"{Root}/{id}";
        }
    }
}
