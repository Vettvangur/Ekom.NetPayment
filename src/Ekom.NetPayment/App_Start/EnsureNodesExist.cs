using Examine;
using GMO.Vorto.PropertyEditor;
using Our.Umbraco.Vorto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Examine;
using Ekom.NetPayment.Exceptions;
using Umbraco.Web;

namespace Ekom.NetPayment.App_Start
{
    class EnsureNodesExist : IComponent
    {
        const string indexName = "ExternalIndex";

        private readonly ILogger _logger;
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyEditorCollection _propertyEditorCollection;
        private readonly IExamineManager _examineMgr;
        private readonly IUmbracoContextFactory _contextFactory;

        public EnsureNodesExist(
            ILogger logger,
            IContentService contentService,
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditorCollection,
            IExamineManager examineMgr,
            IUmbracoContextFactory contextFactory)
        {
            _logger = logger;
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _propertyEditorCollection = propertyEditorCollection;
            _examineMgr = examineMgr;
            _contextFactory = contextFactory;
        }

        public void Initialize()
        {
            _logger.Debug<EnsureNodesExist>("Ensuring Umbraco nodes exist");

            if (IsEkomProject())
            {
                _logger.Debug<EnsureNodesExist>("Is Ekom project, will use Ekom nodes");
                return;
            }

            if (_examineMgr.TryGetIndex(indexName, out IIndex index))
            {
                var searcher = index.GetSearcher();
                var results = searcher.CreateQuery("content")
                    .NodeTypeAlias("netPaymentProviders")
                    .Execute()
                    ;

                // Assume ready if we find a netPaymentProviders content node
                if (results.Any())
                {
                    return;
                }
            }
            else
            {
                _logger.Error<EnsureNodesExist>($"Unable to find Examine index: {indexName}, could not begin NetPayment nodes creation");
                // If we can't find the externalSearcher, assume user has other fixing to do before worrying about NetPayment
                return;
            }

            if (!_propertyEditorCollection.TryGet("Our.Umbraco.Vorto", out IDataEditor vortoEditor))
            {
                throw new EnsureNodesException(
                    "Unable to find Our.Umbraco.Vorto property editor, failed creating Ekom nodes. Ensure GMO.Vorto.Web is installed.");
            }

            #region Data Types

            var netPaymentDtContainer = EnsureDataTypeContainerExists();

            var textstringDt = _dataTypeService.GetDataType(new Guid("0cc0eba1-9960-42c9-bf9b-60e150b429ae"));
            var textareaDt = _dataTypeService.GetDataType(new Guid("c6bac0dd-4ab9-45b1-8e30-e4b619ee5da3"));

            var perStoreTextDt = EnsureDataTypeExists(new DataType(vortoEditor, netPaymentDtContainer.Id)
            {
                Name = "NetPayment - Textstring - Vorto",
                Configuration = new VortoConfiguration
                {
                    DataType = new DataTypeInfo
                    {
                        Guid = textstringDt.Key,
                        Name = textstringDt.Name,
                        PropertyEditorAlias = textstringDt.EditorAlias,
                    },
                    MandatoryBehaviour = "primary",
                },
            });
            var perStoreTextareaDt = EnsureDataTypeExists(new DataType(vortoEditor, netPaymentDtContainer.Id)
            {
                Name = "NetPayment - Textarea - Vorto",
                Configuration = new VortoConfiguration
                {
                    DataType = new DataTypeInfo
                    {
                        Guid = textareaDt.Key,
                        Name = textareaDt.Name,
                        PropertyEditorAlias = textareaDt.EditorAlias,
                    },
                    MandatoryBehaviour = "primary",
                },
            });

            #endregion

            #region Document Types

            var netPaymentDocTypeContainer = EnsureContainerExists("NetPayment");

            var paymentProviderComposition = EnsureContentTypeExists(
                new ContentType(netPaymentDocTypeContainer.Id)
                {
                    Name = "Payment Provider",
                    Alias = "paymentProvider",

                    PropertyGroups = new PropertyGroupCollection(
                        new List<PropertyGroup>
                        {
                                new PropertyGroup(new PropertyTypeCollection(
                                    true,
                                    new List<PropertyType>
                                    {
                                        new PropertyType(perStoreTextDt, "title")
                                        {
                                            Name = "Title",
                                            Mandatory = true,
                                        },
                                        new PropertyType(perStoreTextDt, "successUrl")
                                        {
                                            Name = "Success Url",
                                            Mandatory = true,
                                        },
                                        new PropertyType(perStoreTextDt, "cancelUrl")
                                        {
                                            Name = "Cancel Url",
                                        },
                                        new PropertyType(perStoreTextDt, "errorUrl")
                                        {
                                            Name = "Error Url",
                                            Mandatory = true,
                                        },
                                        new PropertyType(perStoreTextareaDt, "paymentInfo")
                                        {
                                            Name = "Payment Info",
                                        },
                                        new PropertyType(textstringDt, "basePaymentProvider")
                                        {
                                            Name = "Base Payment Provider",
                                            Description = "Allows payment provider overloading. " +
                                                "F.x. Borgun ISK and Borgun USD nodes with different names and different xml configurations targetting the same base payment provider."
                                        },
                                    }))
                                {
                                    Name = "Settings",
                                },
                                new PropertyGroup(new PropertyTypeCollection(
                                    true,
                                    new List<PropertyType>
                                    {
                                        new PropertyType(textstringDt, "imageUrl")
                                        {
                                            Name = "Image Url",
                                        },
                                    }))
                                {
                                    Name = "Image",
                                },
                        }),
                }
            );

            var netPaymentProvidersCt = EnsureContentTypeExists(new ContentType(netPaymentDocTypeContainer.Id)
            {
                Name = "Payment Providers",
                Alias = "paymentProviders",
                Icon = "icon-bills",
                AllowedAsRoot = true,
            });

            #endregion

            // Disabled until Umbraco fixes content creation in components
            //EnsureContentExists("Greiðslugáttir", netPaymentProvidersCt.Alias);

            _logger.Debug<EnsureNodesExist>("Done");
        }

        private IContent EnsureContentExists(string name, string documentTypeAlias, int parentId = -1)
        {
            // ToDo: check for existence if we ever end up creating more content nodes

            var content = _contentService.Create(name, parentId, documentTypeAlias);

            OperationResult res;
            using (_contextFactory.EnsureUmbracoContext())
            {
                res = _contentService.Save(content);
            }

            if (res.Success)
            {
                _logger.Info<EnsureNodesExist>($"Created content {name}, alias {documentTypeAlias}");
                return content;
            }
            else
            {
                throw new EnsureNodesException($"Unable to SaveAndPublish {name} content with doc type {documentTypeAlias} and parent {parentId}");
            }
        }

        private EntityContainer EnsureDataTypeContainerExists()
        {
            var netPaymentContainer = _dataTypeService.GetContainers("NetPayment", 1).FirstOrDefault();
            if (netPaymentContainer == null)
            {
                var createContainerAttempt = _dataTypeService.CreateContainer(-1, "NetPayment");
                if (createContainerAttempt.Success)
                {
                    _logger.Info<EnsureNodesExist>("Created DataType container");
                    netPaymentContainer = createContainerAttempt.Result.Entity;
                }
                else
                {
                    throw new EnsureNodesException("Unable to create container, failed creating NetPayment Data Types", createContainerAttempt.Exception);
                }
            }

            return netPaymentContainer;
        }

        private IDataType EnsureDataTypeExists(DataType dt)
        {
            var textDt = _dataTypeService.GetDataType(dt.Name);

            if (textDt == null)
            {
                textDt = dt;
                _dataTypeService.Save(textDt);
                _logger.Info<EnsureNodesExist>($"Created DataType {dt.Name}, editor alias {dt.EditorAlias}");
            }

            return textDt;
        }

        private EntityContainer EnsureContainerExists(string name, int level = 1, int parentId = -1)
        {
            var ekmContainer = _contentTypeService.GetContainers(name, level).FirstOrDefault(x => x.ParentId == parentId);
            if (ekmContainer == null)
            {
                var createContainerAttempt = _contentTypeService.CreateContainer(parentId, name);
                if (createContainerAttempt.Success)
                {
                    ekmContainer = createContainerAttempt.Result.Entity;
                    _logger.Info<EnsureNodesExist>("Created content container");
                }
                else
                {
                    throw new EnsureNodesException("Unable to create container, failed creating Ekom nodes", createContainerAttempt.Exception);
                }
            }

            return ekmContainer;
        }

        private IContentType EnsureContentTypeExists(ContentType contentType)
        {
            var ekmContentType = _contentTypeService.Get(contentType.Alias);

            if (ekmContentType == null)
            {
                ekmContentType = contentType;
                _contentTypeService.Save(ekmContentType);
                _logger.Info<EnsureNodesExist>($"Created content type {contentType.Name}, alias {contentType.Alias}");
            }

            return ekmContentType;
        }

        private bool IsEkomProject()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(x => x.GetName().Name == "Ekom");
        }

        public void Terminate() { }
    }
}
