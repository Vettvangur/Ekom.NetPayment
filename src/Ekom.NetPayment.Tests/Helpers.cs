using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Profiling;

namespace Umbraco.NetPayment.Tests
{
    public static class Helpers
    {
        public static HttpContext GetHttpContext()
        {
            var tw = new Mock<TextWriter>();
            var req = new HttpRequest("", "", "");
            var resp = new HttpResponse(tw.Object);

            return new HttpContext(req, resp);
        }

        public static ApplicationContext GetSetAppCtx()
        {
            var appCtx = new ApplicationContext(CacheHelper.CreateDisabledCacheHelper(), new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));
            return ApplicationContext.EnsureContext(appCtx, true);
        }


    }
}
