using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Umbraco.NetPayment.Helpers
{
    public static class URIHelper
    {
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

            throw new Exception("Not well formed Uri");
        }
    }
}
