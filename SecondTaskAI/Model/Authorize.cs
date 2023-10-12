namespace SecondTaskAI
{
    internal static class Authorize
    {
        private static string _pathAuthorizeData = @"data\AuthorizeData.txt";
        internal static string getAuthorizeDataPath() => _pathAuthorizeData;
        internal const int AccessToken = 0;
    }
}
