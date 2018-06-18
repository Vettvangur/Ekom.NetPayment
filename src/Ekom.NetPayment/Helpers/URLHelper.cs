using System;
using System.Web;

namespace Umbraco.NetPayment.Helpers
{
    /// <summary>
    /// URI Helper methods
    /// </summary>
    public static class URIHelper
    {
        /// <summary>
        /// Ensures param is full URI, otherwise adds components using data from Request
        /// </summary>
        /// <param name="uri">absolute or relative uri</param>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string EnsureFullUri(string uri, HttpRequestBase Request)
        {
            if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                return uri;
            }
            else if (Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                var url = Request.Url;
                var basePath = $"{url.Scheme}://{url.Authority}";

                return basePath + uri;
            }

            throw new Exception("Not well formed Uri, verify configured success / error url");
        }
    }
}
