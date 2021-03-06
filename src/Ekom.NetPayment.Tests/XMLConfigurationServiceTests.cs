﻿using Examine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Ekom.NetPayment.Tests
{
    [TestClass]
    public class XMLConfigurationServiceTests
    {
        class XMLCfgSvcMocks
        {
            public XMLConfigurationService xmlConfigSvc;
            public Mock<XMLConfigurationService> xmlConfigSvcMocked;
            public Mock<HttpContextBase> httpContext;
            public Mock<IFileSystem> fs;
            public Settings settings;

            public XMLCfgSvcMocks()
            {
                fs = new Mock<IFileSystem>();
                settings = new Settings();
                httpContext = new Mock<HttpContextBase>();

                xmlConfigSvcMocked = new Mock<XMLConfigurationService>(
                    httpContext.Object,
                    AppCaches.Disabled,
                    settings,
                    fs.Object,
                    Mock.Of<ILogger>(),
                    Mock.Of<IExamineManager>());
            }

            public XMLCfgSvcMocks(IFileSystem _fs)
            {
                settings = new Settings();
                httpContext = new Mock<HttpContextBase>();

                xmlConfigSvcMocked = new Mock<XMLConfigurationService>(
                    httpContext.Object,
                    AppCaches.Disabled,
                    settings,
                    _fs,
                    Mock.Of<ILogger>(),
                    Mock.Of<IExamineManager>());
            }

            public XMLCfgSvcMocks(bool _)
            {
                fs = new Mock<IFileSystem>();
                settings = new Settings();
                httpContext = new Mock<HttpContextBase>();

                xmlConfigSvc = new XMLConfigurationService(
                    httpContext.Object,
                    AppCaches.Disabled,
                    settings,
                    fs.Object,
                    Mock.Of<ILogger>(),
                    Mock.Of<IExamineManager>());
            }
        }

        [TestMethod]
        public void CanParseConfig()
        {
            var xdoc = XDocument.Parse(paymentProviders_config);
            var xmlConfigSvcMocks = new XMLCfgSvcMocks();
            xmlConfigSvcMocks.xmlConfigSvcMocked.Setup(x => x.Configuration).Returns(xdoc);
            var properties = xmlConfigSvcMocks.xmlConfigSvcMocked.Object.GetConfigForPP("testBorgun", "testBorgun");

            Assert.AreEqual(6, properties.Count);
            Assert.AreEqual("16", properties["paymentgatewayid"]);
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

            var properties = xmlConfigSvcMocks.xmlConfigSvcMocked.Object.GetConfigForPP("Valitor", "valitor", multiAttributes);

            Assert.AreEqual(6, properties.Count);
            Assert.AreEqual("12345", properties["verificationcode"]);
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

            xmlConfigSvcMocks.httpContext.Setup(x => x.Server.MapPath(It.IsAny<string>())).Returns(path);

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
            var xmlConfigSvcMocks = new XMLCfgSvcMocks(true);
            var xdoc = XDocument.Parse(paymentProviders_config);

            xmlConfigSvcMocks.xmlConfigSvc.SetConfiguration(xdoc);

            Assert.AreEqual(xmlConfigSvcMocks.settings.PPUmbracoNode, Guid.Parse("8d7c912e-3744-46a0-801f-3e7d3c8a991e"));
        }

        readonly string paymentProviders_config = @"<?xml version=""1.0""?>

            <providers>
             <paymentProvidersNode>8d7c912e-3744-46a0-801f-3e7d3c8a991e</paymentProvidersNode>

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

            </providers>";
    }
}
