using Examine;
using Moq;
using System.IO;
using System.Web;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Security;

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

        public static void RegisterMockedHttpContext(IRegister register)
        {
            register.Register(Mock.Of<HttpContextBase>());
        }
        public static void RegisterMockedUmbracoTypes(IRegister register, IFactory factory)
        {
            register.Register(Mock.Of<ILogger>());
            register.Register(Mock.Of<IExamineManager>());
            register.Register(f => AppCaches.Disabled);
            register.Register(Mock.Of<IUmbracoContextAccessor>());
            register.Register(Mock.Of<IUmbracoDatabaseFactory>());
            register.Register(ServiceContext.CreatePartial());
            register.Register(Mock.Of<IProfilingLogger>());
            register.Register(Mock.Of<IUmbracoContextAccessor>());

            Current.Factory = factory;
        }

        /// <summary>
        /// For some inexplicable reason the following registrations block further registrations from
        /// overriding previous ones. Even for different types, f.x. a .Register<IExamineService>
        /// </summary>
        /// <param name="register"></param>
        /// <param name="factory"></param>
        public static void RegisterUmbracoHelper(IRegister register, IFactory factory)
        {
            var membershipHelper = new MembershipHelper(
                factory.GetInstance<HttpContextBase>(),
                Mock.Of<IPublishedMemberCache>(),
                Mock.Of<MembershipProvider>(),
                Mock.Of<RoleProvider>(),
                Mock.Of<IMemberService>(),
                Mock.Of<IMemberTypeService>(),
                Mock.Of<IUserService>(),
                Mock.Of<IPublicAccessService>(),
                Mock.Of<AppCaches>(),
                Mock.Of<ILogger>());
            var umbHelper = new UmbracoHelper(
                Mock.Of<IPublishedContent>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<ICultureDictionaryFactory>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                Mock.Of<IPublishedContentQuery>(),
                membershipHelper);
            register.Register(umbHelper);
        }
    }
}
