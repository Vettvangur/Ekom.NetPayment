using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Core;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Handles the Payment Providers XML configuration
    /// </summary>
    class XMLConfigurationService
    {
        ILog _log;
        HttpServerUtilityBase _server;
        ApplicationContext _appContext;
        Settings _settings;
        IFileSystem _fs;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="appContext"></param>
        /// <param name="settings"></param>
        /// <param name="fileSystem"></param>
        public XMLConfigurationService(HttpServerUtilityBase server, ApplicationContext appContext, Settings settings, IFileSystem fileSystem)
        {
            _server = server;
            _appContext = appContext;
            _settings = settings;
            _fs = fileSystem;

            var logFac = UnityConfig.GetConfiguredContainer().Resolve<ILogFactory>();
            _log = logFac.GetLogger(typeof(XMLConfigurationService));
        }

        /// <summary>
        /// Tries to get the xml configuration from cache, falls back to file.
        /// </summary>
        public virtual XDocument Configuration
        {
            get
            {
                return _appContext.ApplicationCache.RuntimeCache.GetCacheItem("PPConfig", LoadConfiguration) as XDocument;
            }
        }

        /// <summary>
        /// Load XML configuration from file
        /// </summary>
        private XDocument LoadConfiguration()
        {
            var path = _server.MapPath(_settings.PPConfigPath);
            var configFileExists = _fs.File.Exists(path);

            if (configFileExists)
            {
                return XDocument.Load(path);
            }

            return null;
        }

        /// <summary>
        /// Parses the configuration for a given payment provider and returns all nodes as
        /// key values pairs.
        /// </summary>
        /// <param name="pp">Payment Provider title attribute</param>
        /// <param name="secondaryMatches">Optional collection of attributes and values to match on provider</param>
        public Dictionary<string, string> GetConfigForPP(string pp, Dictionary<string, string> secondaryMatches = null)
        {
            var providers = Configuration.Root.Elements("provider")
                            .Where(x => x.Attribute("title")?.Value == pp);

            if (providers.Count() > 0)
            {
                XElement provider;

                if (secondaryMatches != null)
                {
                    provider = providers.FirstOrDefault(x => secondaryMatches.All(kvp => x.Attribute(kvp.Key)?.Value == kvp.Value));
                }
                else
                {
                    provider = providers.FirstOrDefault();
                }

                if (provider != null)
                {
                    var config = new Dictionary<string, string>();

                    foreach (var element in provider.Elements())
                    {
                        config.Add(element.Name.LocalName, element.Value);
                    }

                    return config;
                }
            }

            return null;
        }

        /// <summary>
        /// This method attempts to read the payment providers node guid from the PaymentProviders.config xml file.
        /// It can handle failures such as no xml config present, no value present and more,
        /// in some cases creating a new configuration file.
        /// </summary>
        /// <param name="ppConfig">PaymentProviders.config XML</param>
        public virtual void SetConfiguration(XDocument ppConfig)
        {
            // No configuration file
            if (ppConfig != null)
            {
                var ppConfigRoot = ppConfig?.Root;

                // Can't find root element
                if (ppConfigRoot != null)
                {
                    var ppNode = ppConfigRoot.Element(_settings.PPUNodeConfElName);

                    // Couldn't find element
                    if (ppNode == null)
                    {
                        ppNode = new XElement(_settings.PPUNodeConfElName)
                        {
                            Value = FindPPContainerNodeKey().ToString()
                        };
                        ppConfigRoot.Add(ppNode);
                        ppConfig.Save(_server.MapPath(_settings.PPConfigPath));
                    }

                    bool bPPNode = Guid.TryParse(ppNode.Value, out Guid ppNodeKey);

                    // Malformed value
                    if (!bPPNode)
                    {
                        ppNodeKey = FindPPContainerNodeKey();
                        ppNode.Value = ppNodeKey.ToString();
                        ppConfig.Save(_server.MapPath(_settings.PPConfigPath));
                    }

                    _settings.PPUmbracoNode = ppNodeKey;
                    return;
                }
            }

            // No file or malformed
            CreateConfigurationXML().Wait();
        }

        private Guid FindPPContainerNodeKey()
        {
            try
            {
                var ctId = _appContext.Services.ContentTypeService.GetContentType(_settings.PPDocumentTypeAlias).Id;
                var key = _appContext.Services.ContentService.GetContentOfContentType(ctId).First().Key;
                return key;
            }
            catch (Exception ex)
            {
                _log.Error("Unable to find payment provider node, please verify document type alias and umbraco node presence.", ex);
                throw;
            }
        }

        private async Task CreateConfigurationXML()
        {
            var path = _server.MapPath(_settings.PPConfigPath);
            var nodeKey = FindPPContainerNodeKey().ToString();

            await WriteXMLAsync(path, nodeKey);
        }

        static XmlWriterSettings xmlWrSettings = new XmlWriterSettings
        {
            Async = true,
            Indent = true,
            NewLineOnAttributes = true,
        };

        /// <summary>
        /// This was made async for shits and giggles.
        /// Needs configureAwait if to be used outside of startup method.
        /// </summary>
        private async Task WriteXMLAsync(string path, string nodeKey)
        {
            var dirPath = _fs.Path.GetDirectoryName(path);
            _fs.Directory.CreateDirectory(dirPath);

            using (var xmlWriter = XmlWriter.Create(path, xmlWrSettings))
            {
                await xmlWriter.WriteStartDocumentAsync();
                await xmlWriter.WriteStartElementAsync("", "providers", "");
                await xmlWriter.WriteStartElementAsync("", _settings.PPUNodeConfElName, "");
                await xmlWriter.WriteStringAsync(nodeKey);
            }
        }
    }
}
