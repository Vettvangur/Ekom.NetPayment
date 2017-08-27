using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umbraco.Web;

namespace Umbraco.NetPayment.Tests
{
	[TestClass]
	public class UmbracoServiceTests
	{
		class UmbracoServiceMocks
		{
			public UmbracoService umbracoSvc;
			//public Mock<UmbracoService> umbracoSvcMocked;
			UmbracoHelper _umbracoHelper;
			public Settings settings;

			public UmbracoServiceMocks(bool unMocked)
			{
				settings = new Settings();
				var uHelperMocks = new UHelperMocks();

				_umbracoHelper = uHelperMocks.uHelper;
				umbracoSvc = new UmbracoService(_umbracoHelper, settings);
			}
		}

		// Needs mock for all methods called here:
		// https://github.com/umco/umbraco-vorto/blob/master/src/Our.Umbraco.Vorto/Extensions/IPublishedContentExtensions.cs
		// Update: probably impossible....
		//[TestMethod]
		//public void GetsStringVortoValues()
		//{
		//    var cacheMocks = new CacheMocks();
		//    var appCtx = new ApplicationContext(cacheMocks.cacheHelper, new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));
		//    ApplicationContext.EnsureContext(appCtx, true);
		//    var propValueConverter = new Mock<ManyObjectsResolverBase<PropertyValueConvertersResolver, IPropertyValueConverter>>();
		//    PropertyValueConvertersResolver.Current = (PropertyValueConvertersResolver) propValueConverter.Object;

		//    var dtd = new Mock<IDataTypeDefinition>();

		//    cacheMocks.runtimeCache.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>())).Returns(dtd.Object);

		//    var propertyDic = new Dictionary<string, string>
		//    {
		//        { "propKey", "" }
		//    };

		//    var vortoCultureValue = new Dictionary<string, object>
		//    {

		//        { "is-IS", "propValue" }
		//    };

		//    var vortoValue = new VortoValue
		//    {
		//        DtdGuid = Guid.Parse("8001d413-aee2-4586-952b-cf0d7e021de5"),
		//        Values = vortoCultureValue,
		//    };

		//    var prop = new Mock<IPublishedProperty>();
		//    prop.Setup(x => x.PropertyTypeAlias).Returns("propKey");
		//    prop.Setup(x => x.Value).Returns(vortoValue);
		//    prop.Setup(x => x.HasValue).Returns(true);

		//    var content = new Mock<IPublishedContent>();
		//    //content.Setup(x => x.HasValue(It.Is<string>(y => y == "propKey"))).Returns(true);
		//    content.Setup(x => x.GetProperty(It.Is<string>(y => y == "propKey"))).Returns(prop.Object);
		//    content.Setup(x => x.GetProperty(It.Is<string>(y => y == "propKey"), It.IsAny<bool>())).Returns(prop.Object);

		//    var uSvc = new UmbracoServiceMocks(true);

		//    var dic = uSvc.umbracoSvc.GetPPProperties(content.Object, "is", propertyDic);

		//    Assert.AreEqual(dic["propKey"], "propValue");
		//}

		//[TestMethod]
		//public void GetsIdFromContentVortoValues()
		//{
		//    var propertyDic = new Dictionary<string, string>
		//    {
		//        { "propKey", "" }
		//    };

		//    var contentValue = new Mock<IPublishedContent>();
		//    contentValue.Setup(x => x.ItemType).Returns(PublishedItemType.Content);
		//    contentValue.Setup(x => x.Id).Returns(30);

		//    var prop = new Mock<IPublishedProperty>();
		//    prop.Setup(x => x.PropertyTypeAlias).Returns("propKey");
		//    prop.Setup(x => x.Value).Returns(contentValue.Object);

		//    var content = new Mock<IPublishedContent>();
		//    content.Setup(x => x.HasVortoValue(
		//        It.Is<string>(y => y == "propKey"), 
		//        It.IsAny<string>(),
		//        false, null
		//    )).Returns(true);

		//    content.Setup(x => x.GetVortoValue(
		//        It.Is<string>(y => y == "propKey"), 
		//        It.IsAny<string>(),
		//        false, null, null
		//    )).Returns(contentValue);

		//    var uSvc = new UmbracoServiceMocks(true);

		//    var dic = uSvc.umbracoSvc.GetPPProperties(content.Object, "is", propertyDic);

		//    Assert.AreEqual(dic["propKey"], "30");
		//}
	}
}
