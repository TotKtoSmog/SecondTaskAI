namespace SecondTaskAI
{
    internal static class Authorize
    {
        private static string _pathAuthorizeData = @"data\AuthorizeData.txt";
        internal static string GetAuthorizeDataPath() => _pathAuthorizeData;
        internal const int AccessToken = 0;
    }
}
