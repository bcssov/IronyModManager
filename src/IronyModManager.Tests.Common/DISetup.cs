// ***********************************************************************
// Assembly         : IronyModManager.Tests.Common
// Author           : Mario
// Created          : 01-31-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="DISetup.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
                    PluginPathAndName = Constants.PluginsPathAndName
                });
        }

        #endregion Methods
    }
}
