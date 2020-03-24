// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 03-24-2020
// ***********************************************************************
// <copyright file="DIPackage.Views.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Views;
using IronyModManager.Views.Controls;
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
        /// Registers the views.
        /// </summary>
        /// <param name="container">The container.</param>
        private void RegisterViews(Container container)
        {
            container.Register<MainWindow>();
            container.Register<ThemeControlView>();
            container.Register<LanguageControlView>();
            container.Register<MainControlView>();
            container.Register<GameControlView>();
            container.Register<InstalledModsControlView>();
            container.Register<SortOrderControlView>();
            container.Register<ModHolderControlView>();
            container.Register<SearchModsControlView>();
            container.Register<CollectionModsControlView>();
            container.Register<AddNewCollectionControlView>();
            container.Register<ExportModCollectionControlView>();
            container.Register<MainConflictSolverControlView>();
            container.Register<MergeViewerControlView>();
            container.Register<ModCompareSelectorControlView>();
        }

        #endregion Methods
    }
}
