// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="DIPackage.Avalonia.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Container = SimpleInjector.Container;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIPackage.
    /// </summary>
    public partial class DIPackage
    {
        #region Methods

        /// <summary>
        /// Registers the avalonia services.
        /// </summary>
        /// <param name="container">The container.</param>
        private void RegisterAvaloniaServices(Container container)
        {
            // Have to manually bind Avalonia services... it doesn't really work best with SimpleInjector...
            container.Register<IActivationForViewFetcher, AvaloniaActivationForViewFetcher>();
            container.Register<IPropertyBindingHook, AutoDataTemplateBindingHook>();
        }

        #endregion Methods
    }
}
