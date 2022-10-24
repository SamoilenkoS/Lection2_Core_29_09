using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Services.QueryService
{
    public interface IQueryService
    {
        Dictionary<string, string> CreateQueryParams(string email, string emailKey);
        void AddQueryParamsToUri(UriBuilder uriBuilder, Dictionary<string, string> queryParams);
    }
}
