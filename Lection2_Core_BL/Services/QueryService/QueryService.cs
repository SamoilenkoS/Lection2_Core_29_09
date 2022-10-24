namespace Lection2_Core_BL.Services.QueryService
{
    public class QueryService : IQueryService
    {
        public void AddQueryParamsToUri(UriBuilder uriBuilder, Dictionary<string, string> queryParams)
        {
            uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        public Dictionary<string, string> CreateQueryParams(string email, string emailKey)
        {
            var queryParams = new Dictionary<string, string>();
            queryParams.Add("email", email);
            queryParams.Add("key", emailKey);
            return queryParams;
        }
    }
}
