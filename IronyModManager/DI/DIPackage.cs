// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Shared;
using ReactiveUI;
using SimpleInjector;
using SimpleInjector.Packaging;
using Splat;
using Splat.SimpleInjector;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public partial class DIPackage : IPackage
    {
        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            var resolver = new SimpleInjectorDependencyResolver(container);
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            RxApp.MainThreadScheduler = Avalonia.Threading.AvaloniaScheduler.Instance;

            RegisterAvaloniaServices(container);
            RegisterReactiveServices(container);
            RegisterViews(container);
            RegisterViewModels(container);
            RegisterImplementations(container);
        }

        #endregion Methods
    }
}
