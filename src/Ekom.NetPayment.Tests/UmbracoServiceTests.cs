using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Our.Umbraco.Vorto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Umbraco.NetPayment.Tests
{
    [TestClass]
    public class UmbracoServiceTests
    {
        class UmbracoServiceMocks
        {
            public UmbracoService umbracoSvc;
            public Mock<UmbracoService> umbracoSvcMocked;
            UmbracoHelper _umbracoHelper;
            public Settings settings;

            public UmbracoServiceMocks(bool unMocked)
            {
                settings = new Settings();
                _umbracoHelper = Helpers.GetUHelper();
                umbracoSvc = new UmbracoService(_umbracoHelper, settings);
            }
        }

        // Needs mock for all methods called here:
        // https://github.com/umco/umbraco-vorto/blob/master/src/Our.Umbraco.Vorto/Extensions/IPublishedContentExtensions.cs
        //[TestMethod]
        //public void GetsStringVortoValues()
        //{
        //    var propertyDic = new Dictionary<string, string>
        //    {
        //        { "propKey", "" }
        //    };

        //    var prop = new Mock<IPublishedProperty>();
        //    prop.Setup(x => x.PropertyTypeAlias).Returns("propKey");

        //    var content = new Mock<IPublishedContent>();
        //    content.Setup(x => x.HasVortoValue(
        //        It.Is<string>(y => y == "propKey"), 
        //        It.IsAny<string>(), 
        //        false, null
        //    )).Returns(true);
        //    Our.Umbraco.Vorto.Models.VortoValue();
        //    content.Setup(x => x.GetVortoValue(
        //        It.Is<string>(y => y == "propKey"), 
        //        It.IsAny<string>(), 
        //        false, null, null)
        //    ).Returns("propValue");

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
