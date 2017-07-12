using Umbraco.Core;
using log4net;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Practices.Unity;

namespace Umbraco.NetPayment
{
    public class UmbEvents : ApplicationEventHandler
    {
        UmbracoApplicationBase _umbracoApplication;
        ApplicationContext _applicationContext;
        Settings _settings;

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            _settings = UnityConfig.GetConfiguredContainer().Resolve<Settings>();
        }

        private XDocument LoadConfiguration()
        {
            var configFileExists = System.IO.File.Exists(configFile);

            if (configFileExists)
            {
                return XDocument.Load(configFile);
            }
        }

        private object F()
        {
            if (ppConfig != null)
            {
                var ppNode = ppConfig?.FirstNode?.Document?.Element("paymentProvidersNode")?.Value;

                if (ppNode != null)
                {
                    bool bPPNode = int.TryParse(ppNode, out int ppNodeId);

                    if (bPPNode)
                    {
                        settings.PPUmbracoNode = ppNodeId;
                    }
                }
            }
        }

        private object F2()
        {
            var ctId = _applicationContext.Services.ContentTypeService.GetContentType(_settings.PPDocumentTypeAlias).Id;
            var id = _applicationContext.Services.ContentService.GetContentOfContentType(ctId).First().Id;
        }
    }
}
