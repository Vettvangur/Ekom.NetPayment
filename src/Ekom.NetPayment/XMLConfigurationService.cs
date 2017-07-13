﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
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
    public class XMLConfigurationService
    {
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
            if (ppConfig != null)
            {
                var ppConfigRoot = ppConfig?.Root;

                // Can't find root element
                if (ppConfigRoot == null) throw new Exception("Malformed configuration xml");

                var ppNode = ppConfigRoot.Element(_settings.PPUNodeConfElName);

                if (ppNode != null)
                {
                    bool bPPNode = Guid.TryParse(ppNode.Value, out Guid ppNodeId);

                    if (bPPNode)
                    {
                        _settings.PPUmbracoNode = ppNodeId;
                        return;
                    }

                    ppNode.Value = FindPPContainerNodeKey().ToString();
                    return;
                }

                // Couldn't find element
                var newPPNode = new XElement(_settings.PPUNodeConfElName)
                {
                    Value = FindPPContainerNodeKey().ToString()
                };
                ppConfigRoot.Add(newPPNode);
                ppConfig.Save(_settings.PPConfigPath);
            }

            // No file
            CreateConfigurationXML().Wait();
        }

        private Guid FindPPContainerNodeKey()
        {
            var ctId = _appContext.Services.ContentTypeService.GetContentType(_settings.PPDocumentTypeAlias).Id;
            var key = _appContext.Services.ContentService.GetContentOfContentType(ctId).First().Key;
            return key;
        }

        private async Task CreateConfigurationXML()
        {
            var path = _server.MapPath(_settings.PPConfigPath);
            var nodeKey = FindPPContainerNodeKey().ToString();

            await WriteXMLAsync(path, nodeKey).ConfigureAwait(false);
        }

        static XmlWriterSettings xmlWrSettings = new XmlWriterSettings
        {
            Async = true,
        };

        private async Task WriteXMLAsync(string path, string nodeKey)
        {
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
