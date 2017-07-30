using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.IO;
using System.IO.Abstractions;
using Moq;
using System.Reflection;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Umbraco.NetPayment.Tests
{
    [TestClass]
    public class XMLConfigurationServiceTests
    {
        class XMLCfgSvcMocks
        {
            public XMLConfigurationService xmlConfigSvc;
            public Mock<XMLConfigurationService> xmlConfigSvcMocked;
            public Mock<HttpServerUtilityBase> server;
            public Mock<IFileSystem> fs;
            public Settings settings;

            public XMLCfgSvcMocks()
            {
                fs = new Mock<IFileSystem>();
                settings = new Settings();
                server = new Mock<HttpServerUtilityBase>();

                xmlConfigSvcMocked = new Mock<XMLConfigurationService>(server.Object, Helpers.GetSetAppCtx(), settings, fs.Object);
            }

            public XMLCfgSvcMocks(IFileSystem _fs)
            {
                settings = new Settings();
                server = new Mock<HttpServerUtilityBase>();

                xmlConfigSvcMocked = new Mock<XMLConfigurationService>(server.Object, Helpers.GetSetAppCtx(), settings, _fs);
            }
            
            public XMLCfgSvcMocks(bool unMocked)
            {
                fs = new Mock<IFileSystem>();
                settings = new Settings();
                server = new Mock<HttpServerUtilityBase>();
                
                xmlConfigSvc = new XMLConfigurationService(server.Object, Helpers.GetSetAppCtx(), settings, fs.Object);
            }
        }

        [TestMethod]
        public void CanParseConfig()
        {
            var xdoc = XDocument.Parse(paymentProviders_config);
            var xmlConfigSvcMocks = new XMLCfgSvcMocks();
            xmlConfigSvcMocks.xmlConfigSvcMocked.Setup(x => x.Configuration).Returns(xdoc);
            var properties = xmlConfigSvcMocks.xmlConfigSvcMocked.Object.GetConfigForPP("Borgun");

            Assert.AreEqual(properties.Count, 6);
            Assert.AreEqual(properties["paymentgatewayid"], "3156");
        }

        [TestMethod]
        public void HandlesMultipleAttributeMatching()
        {
            var xdoc = XDocument.Parse(paymentProviders_config);
            var xmlConfigSvcMocks = new XMLCfgSvcMocks();
            xmlConfigSvcMocks.xmlConfigSvcMocked.Setup(x => x.Configuration).Returns(xdoc);

            var multiAttributes = new Dictionary<string, string>
            {
                { "store", "EN" },
                { "language", "en" },
            };

            var properties = xmlConfigSvcMocks.xmlConfigSvcMocked.Object.GetConfigForPP("Valitor", multiAttributes);

            Assert.AreEqual(properties.Count, 6);
            Assert.AreEqual(properties["verificationcode"], "12345");
        }

        [TestMethod]
        public void CreatesReadableConfig()
        {
            var xmlConfigSvcMocks = new XMLCfgSvcMocks(new FileSystem());
            var xmlConfigSvc = xmlConfigSvcMocks.xmlConfigSvcMocked.Object;
            string path = Directory.GetCurrentDirectory() + "\\tests\\test.xml";

            var task = new PrivateObject(
                xmlConfigSvc, 
                new PrivateType(
                    typeof(XMLConfigurationService)
                )
            ).Invoke(
                "WriteXMLAsync", 
                new object[] { path, "12345" }
            ) as Task;

            task.Wait();

            xmlConfigSvcMocks.server.Setup(x => x.MapPath(It.IsAny<string>())).Returns(path);

            var xdoc = new PrivateObject(
                xmlConfigSvc,
                new PrivateType(
                    typeof(XMLConfigurationService)
                )
            ).Invoke(
                "LoadConfiguration",
                null
            ) as XDocument;

            File.Delete(path);

            var val = xdoc?.Root?.Element("paymentProvidersNode")?.Value;
            Assert.AreEqual(val, "12345");
        }

        [TestMethod]
        public void SetsConfigurationCorrectly()
        {
            var xmlConfigSvcMocks = new XMLCfgSvcMocks(unMocked: true);
            var xdoc = XDocument.Parse(paymentProviders_config);

            xmlConfigSvcMocks.xmlConfigSvc.SetConfiguration(xdoc);

            Assert.AreEqual(xmlConfigSvcMocks.settings.PPUmbracoNode, Guid.Parse("8d7c912e-3744-46a0-801f-3e7d3c8a991e"));
        }

        string paymentProviders_config = @"<?xml version=""1.0""?>

            <providers>
             <paymentProvidersNode>8d7c912e-3744-46a0-801f-3e7d3c8a991e</paymentProvidersNode>
              <provider title=""Borgun"">
                <provider>borgun</provider>
                <merchantid>9909725</merchantid>
                <secretcode>abfa29a72e7c41ff89f3fdfee72d11fc</secretcode>
                <paymentgatewayid>3156</paymentgatewayid>
                <url>https://securepay.borgun.is/securepay/default.aspx</url>
                <testURL>https://securepay.borgun.is/securepay/default.aspx</testURL>
              </provider>

	            <provider title=""BorgunLoans"">
		            <merchantid>9909725</merchantid>
		            <username>FranchMichelsenrad</username>
		            <password>FhxbgN9g8G5jp56Hgzi440jStCn509</password>
		            <url>https://radgreidslur.borgun.is/Login/Token/</url>
		            <testURL>https://radgreidslur.borgun.is/Login/Token/</testURL>
	            </provider>
		
              <provider title=""Netgiro"">
		            <provider>netgiro</provider>
		            <applicationid>87133bfc-93ba-482e-bbcc-a70d1978e29f</applicationid>
		            <iframe>false</iframe>
		            <secretkey>RGgFHirWD8qDyoq6G6wnfsIvPdiB85KwGIkaBevpZ57ENhq/IswTmrVR9XcInqRkR6X30iKjAYSdLdBaPMKSi60f96SjB/fhL7WXHJk9FWQ6A7EV9EuFy6cexVshqKWh4856xPb3ynNJEpVz1FrFT2BkYtqdHddzYMf9I25h140=</secretkey>
		            <url>https://securepay.netgiro.is/v1/</url>
		            <testURL>http://test.netgiro.is/user/securepay/</testURL>
	            </provider>

              <provider title=""Pei"">
                <provider>pei</provider>
                <clientId>michelsen</clientId>
                <secret>BYKadJSZ6OwWEI8s9aKi</secret>
                <merchantid>1077</merchantid>
                <portalUrl>https://gattin.pei.is/</portalUrl>
                <apiUrl>https://api.pei.is/</apiUrl>
                <tokenEndpointUrl>https://auth.pei.is/core/connect/token/</tokenEndpointUrl>
              </provider>
  
              <provider title=""testBorgun"">
                <provider>borgun</provider>
                <merchantid>9275444</merchantid>
                <secretcode>99887766</secretcode>
                <paymentgatewayid>16</paymentgatewayid>
                <url>https://test.borgun.is/SecurePay/default.aspx</url>
                <testURL>https://test.borgun.is/SecurePay/default.aspx</testURL>
              </provider>
  
              <provider title=""Valitor"" store=""IS"" language=""is"">
                <provider>valitor</provider>
                <paymentsuccessfuluurltext>Til baka</paymentsuccessfuluurltext>
                <merchantid>1</merchantid>
                <verificationcode>12345</verificationcode>
                <url>https://testgreidslusida.valitor.is</url>
                <testURL>https://testgreidslusida.valitor.is</testURL>
              </provider>
    
              <provider title=""Valitor"" store=""EN"" language=""en"">
                <provider>valitor</provider>
                <paymentsuccessfuluurltext>Til baka</paymentsuccessfuluurltext>
                <merchantid>1</merchantid>
                <verificationcode>12345</verificationcode>
                <url>https://testgreidslusida.valitor.is</url>
                <testURL>https://testgreidslusida.valitor.is</testURL>
              </provider>

              <provider title=""Greitt"" store=""IS"">
                <provider>greitt</provider>
                <authorization>Basic YnVkaW5AYnVkaW4uaXM6MTIzNDU2</authorization>
                <apiKey>5eB2jSklLFooTQ2BY8DuVPuiRs23CWeG</apiKey>
                <liveUrl>https://api.greitt.is/v1/</liveUrl>
                <testUrl>https://private-anon-b0c26e14d-greitt.apiary-mock.com/v1/</testUrl>
              </provider>


            </providers>";
    }
}
