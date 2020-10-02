// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 10-02-2020
// ***********************************************************************
// <copyright file="DIPackage.Avalonia.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using Avalonia.ReactiveUI;
using IronyModManager.Log;
using ReactiveUI;
using Splat;
using Container = SimpleInjector.Container;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    public partial class DIPackage
    {
        #region Methods

        /// <summary>
        /// Registers the avalonia services.
        /// </summary>
        /// <param name="container">The container.</param>
        private void RegisterAvaloniaServices(Container container)
        {
            var viewFetcher = new AvaloniaActivationForViewFetcher();
            var dataTemplate = new AutoDataTemplateBindingHook();
            Locator.CurrentMutable.RegisterConstant(viewFetcher, typeof(IActivationForViewFetcher));
            Locator.CurrentMutable.RegisterConstant(dataTemplate, typeof(IPropertyBindingHook));
            container.RegisterInstance<IActivationForViewFetcher>(viewFetcher);
            container.RegisterInstance<IPropertyBindingHook>(dataTemplate);
            Avalonia.Logging.Logger.Sink = new AvaloniaLogger();
        }

        #endregion Methods
    }
}
