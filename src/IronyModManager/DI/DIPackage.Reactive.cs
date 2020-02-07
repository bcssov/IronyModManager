// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="DIPackage.Reactive.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using ReactiveUI;
using SimpleInjector.Diagnostics;
using Splat;
using Splat.NLog;
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
        private void RegisterReactiveServices(Container container)
        {
            Locator.CurrentMutable.UseNLogWithWrappingFullLogger();
            container.Register<ViewModelActivator>();
            var viewModelActivatorRegistration = container.GetRegistration(typeof(ViewModelActivator)).Registration;
            viewModelActivatorRegistration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Error in detection.");
        }

        #endregion Methods
    }
}
