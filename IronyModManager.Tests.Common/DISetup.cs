// ***********************************************************************
// Assembly         : IronyModManager.Tests.Common
// Author           : Mario
// Created          : 01-31-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2020
// ***********************************************************************
// <copyright file="DISetup.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.DI;
using IronyModManager.Shared;
using SimpleInjector;

namespace IronyModManager.Tests.Common
{
    /// <summary>
    /// Class DIContainer.
    /// </summary>
    public static class DISetup
    {
        #region Methods

        /// <summary>
        /// Setups the container.
        /// </summary>
        public static void SetupContainer()
        {
            var container = new Container();
            Bootstrap.Setup(
                new DIOptions()
                {
                    Container = container,
                    PluginPathAndName = Constants.PluginsPathAndName,
                    ModuleTypes = new List<Type>() { typeof(IModule) },
                    PluginTypes = new List<Type>() { typeof(IPlugin) }
                });
        }

        #endregion Methods
    }
}
