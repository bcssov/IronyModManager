// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2021
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
        /// <summary>
        /// Registers the avalonia services.
        /// </summary>
        /// <param name="container">The container.</param>
#pragma warning disable CA1822 // Mark members as static

        #region Methods

        private void RegisterAvaloniaServices(Container container)
#pragma warning restore CA1822 // Mark members as static
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
