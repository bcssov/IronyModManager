// ***********************************************************************
// Assembly         : IronyModManager.Tests.Common
// Author           : Mario
// Created          : 01-31-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
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
        #region Properties

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public static Container Container { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Setups the container.
        /// </summary>
        public static void SetupContainer()
        {
            Container = new Container();
            Bootstrap.Setup(
                new DIOptions()
                {
                    Container = Container,
                    PluginPathAndName = Constants.PluginsPathAndName,
                    ModuleTypes = new List<Type>() { typeof(IModule) },
                    PluginTypes = new List<Type>() { typeof(IPlugin) }
                });
        }

        #endregion Methods
    }
}
