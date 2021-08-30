﻿// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 08-29-2021
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.DI.Extensions;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.DLC;
using IronyModManager.IO.Common.Game;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Common.Updater;
using IronyModManager.IO.DLC;
using IronyModManager.IO.Game;
using IronyModManager.IO.Models;
using IronyModManager.IO.Mods;
using IronyModManager.IO.Mods.Models;
using IronyModManager.IO.Readers;
using IronyModManager.IO.Updater;
using IronyModManager.Shared;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public class DIPackage : IPackage
    {
        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.Collection.Register(typeof(IFileReader), typeof(DIPackage).Assembly);
            container.Register<IFileInfo, FileInfo>();
            container.Register<IReader, Reader>();
            container.Register<IModCollectionExporter, ModCollectionExporter>();
            container.Register<IModWriter, ModWriter>();
            container.Register<IModPatchExporter, ModPatchExporter>();
            container.Collection.Register(typeof(IDefinitionInfoProvider), typeof(DIPackage).Assembly);
            container.Register<IPatchState, PatchState>();
            container.Register<IModMergeExporter, ModMergeExporter>();
            container.Register<IUnpacker, Unpacker>();
            container.Register<IReportExporter, ReportExporter>();
            container.Register<IModMergeCompressExporter, ModMergeCompressExporter>();
            container.Register<ITempFile, TempFile.TempFile>();
            container.RemoveTransientWarning<ITempFile>();
            container.Register<IDLCExporter, DLCExporter>();
            container.Register<IDriveInfoProvider, DriveInfoProvider>();
            container.Register<IGameIndexer, GameIndexer>();
            container.Register<ICollectionImportResult, CollectionImportResult>();
        }

        #endregion Methods
    }
}
