using Examine.SearchCriteria;
using log4net;
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
    public class XMLConfigurationService : IXMLConfigurationService
    {
        /// <summary>
        /// </summary>
        public static IXMLConfigurationService Instance =>
            Settings.container.GetInstance<IXMLConfigurationService>();

        readonly ILog _log;
        readonly HttpServerUtilityBase _server;
        readonly ApplicationContext _appContext;
        readonly Settings _settings;
        readonly IFileSystem _fs;
        readonly ExamineManagerBase _examineManager;
        /// <summary>
        /// ctor
        /// </summary>
        internal XMLConfigurationService(
            HttpServerUtilityBase server,
            ApplicationContext appContext,
            Settings settings,
            IFileSystem fileSystem,
            ILogFactory logFac,
            ExamineManagerBase examineManagerBase
        )
        {
            _server = server;
            _appContext = appContext;
            _settings = settings;
            _fs = fileSystem;
            _examineManager = examineManagerBase;

            _log = logFac.GetLogger(typeof(XMLConfigurationService));
        }

        /// <inheritdoc />
        public virtual XDocument Configuration => _appContext.ApplicationCache.RuntimeCache.GetCacheItem("PPConfig", LoadConfiguration) as XDocument;

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
        /// <param name="secondaryMatches">
        /// Optional collection of attributes and values to match on provider element.
        /// F.x. lang
        /// Currently unused, missing a clever way to pass secondary matches on to response callbacks.
        /// </param>
        public Dictionary<string, string> GetConfigForPP(string pp, Dictionary<string, string> secondaryMatches = null)
        {
            var providers = Configuration.Root.Elements("provider")
                            .Where(x => x.Attribute("title")?.Value.ToLower() == pp)
                            .ToList();

            if (providers.Any())
            {
                XElement provider;

                if (secondaryMatches != null)
                {
                    provider = providers.FirstOrDefault(x => secondaryMatches.All(kvp => x.Attribute(kvp.Key)?.Value == kvp.Value))
                        ?? providers.FirstOrDefault();
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
        internal virtual void SetConfiguration(XDocument ppConfig)
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
            var searcher = _examineManager.SearchProviderCollection["ExternalSearcher"];

            ISearchCriteria searchCriteria = searcher.CreateSearchCriteria();
            var query = searchCriteria.NodeTypeAlias(_settings.PPDocumentTypeAlias);
            var results = searcher.Search(query.Compile());

            try
            {
                return Guid.Parse(results.First().Fields["key"]);
            }
            catch (InvalidOperationException ex)
            {
                _log.Error($"Unable to find payment provider node with docTypeAlias {_settings.PPDocumentTypeAlias}, please verify document type alias and umbraco node presence.", ex);
                throw;
            }
        }

        private async Task CreateConfigurationXML()
        {
            var path = _server.MapPath(_settings.PPConfigPath);
            var nodeKey = FindPPContainerNodeKey();

            if (nodeKey != Guid.Empty)
            {
                await WriteXMLAsync(path, nodeKey.ToString());
            }
        }

        static readonly XmlWriterSettings xmlWrSettings = new XmlWriterSettings
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
