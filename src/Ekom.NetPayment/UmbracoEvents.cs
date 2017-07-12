﻿using Umbraco.Core;
using log4net;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Practices.Unity;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Hooks into the umbraco application startup lifecycle 
    /// </summary>
    public class UmbEvents : ApplicationEventHandler
    {
        UmbracoApplicationBase _umbracoApplication;
        ApplicationContext _applicationContext;
        Settings _settings;

        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            _umbracoApplication = umbracoApplication;
            _applicationContext = applicationContext;
            _settings = UnityConfig.GetConfiguredContainer().Resolve<Settings>();
            var xmlConfigService = UnityConfig.GetConfiguredContainer().Resolve<XMLConfigurationService>();

            var url = umbracoApplication.Context.Request.Url;
            _settings.BasePath = $"{url.Scheme}://{url.Authority}";

            // PaymentProviders.config
            var doc = xmlConfigService.Configuration;
            SetConfiguration(doc);
        }


    }
}
