using Examine;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Examine;
using Ekom.NetPayment.Exceptions;

namespace Ekom.NetPayment
{
    /// <summary>
    /// Handles the Payment Providers XML configuration
    /// </summary>
    public class XMLConfigurationService : IXMLConfigurationService
    {
        /// <summary>
        /// </summary>
        public static IXMLConfigurationService Instance =>
            Current.Factory.GetInstance<IXMLConfigurationService>();

        readonly ILogger _logger;
        readonly HttpContextBase _httpContext;
        readonly AppCaches _appCaches;
        readonly Settings _settings;
        readonly IFileSystem _fs;
        readonly IExamineManager _examineManager;
        /// <summary>
        /// ctor
        /// </summary>
        internal XMLConfigurationService(
            HttpContextBase httpContext,
            AppCaches appCaches,
            Settings settings,
            IFileSystem fileSystem,
            ILogger logger,
            IExamineManager examineManager
        )
        {
            _httpContext = httpContext;
            _appCaches = appCaches;
            _settings = settings;
            _fs = fileSystem;
            _examineManager = examineManager;

            _logger = logger;
        }

        /// <inheritdoc />
        public virtual XDocument Configuration => _appCaches.RuntimeCache.GetCacheItem("PPConfig", LoadConfiguration) as XDocument;

        /// <summary>
        /// Load XML configuration from file
        /// </summary>
        private XDocument LoadConfiguration()
        {
            var path = _httpContext.Server.MapPath(_settings.PPConfigPath);
            var configFileExists = _fs.File.Exists(path);

            if (configFileExists)
            {
                return XDocument.Load(path);
            }

            return null;
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetConfigForPP(
            string pp, 
            string basePPName,
            Dictionary<string, string> secondaryMatches = null)
        {
            var providers = Configuration.Root.Elements("provider")
                            .Where(x => x.Attribute("title")?.Value.ToLower() == pp.ToLower())
                            .ToList();

            if (!providers.Any())
            {
                providers = Configuration.Root.Elements("provider")
                            .Where(x => x.Attribute("title")?.Value.ToLower() == basePPName?.ToLower())
                            .ToList();
            }

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
                        ppConfig.Save(_httpContext.Server.MapPath(_settings.PPConfigPath));
                    }

                    bool bPPNode = Guid.TryParse(ppNode.Value, out Guid ppNodeKey);

                    // Malformed value
                    if (!bPPNode)
                    {
                        ppNodeKey = FindPPContainerNodeKey();
                        ppNode.Value = ppNodeKey.ToString();
                        ppConfig.Save(_httpContext.Server.MapPath(_settings.PPConfigPath));
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
            if (_examineManager.TryGetIndex("ExternalIndex", out IIndex extIdx))
            {
                try
                {
                    var results = extIdx.GetSearcher()
                        .CreateQuery("content")
                        .NodeTypeAlias(_settings.PPDocumentTypeAlias)
                        .Execute();

                    return Guid.Parse(results.First().Values["__Key"]);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.Error<XMLConfigurationService>(
                        ex,
                        $"Unable to find payment provider node with docTypeAlias {_settings.PPDocumentTypeAlias}, please verify document type alias and umbraco node presence."
                    );
                    throw;
                }
            }
            else
            {
                throw new ExternalIndexNotFound(
                    "Unable to open ExternalIndex"
                );
            }
        }

        private async Task CreateConfigurationXML()
        {
            var path = _httpContext.Server.MapPath(_settings.PPConfigPath);
            var nodeKey = FindPPContainerNodeKey();

            if (nodeKey != Guid.Empty)
            {
                await WriteXMLAsync(path, nodeKey.ToString()).ConfigureAwait(false);
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
        /// </summary>
        private async Task WriteXMLAsync(string path, string nodeKey)
        {
            var dirPath = _fs.Path.GetDirectoryName(path);
            _fs.Directory.CreateDirectory(dirPath);

            using (var xmlWriter = XmlWriter.Create(path, xmlWrSettings))
            {
                await xmlWriter.WriteStartDocumentAsync().ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync("", "providers", "").ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync("", _settings.PPUNodeConfElName, "").ConfigureAwait(false);
                await xmlWriter.WriteStringAsync(nodeKey).ConfigureAwait(false);
            }
        }
    }
}
