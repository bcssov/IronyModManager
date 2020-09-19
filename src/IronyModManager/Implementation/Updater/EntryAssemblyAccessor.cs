// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 09-20-2020
// ***********************************************************************
// <copyright file="EntryAssemblyAccessor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Diagnostics;
using NetSparkleUpdater.Interfaces;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Class EntryAssemblyAccessor.
    /// Implements the <see cref="NetSparkleUpdater.Interfaces.IAssemblyAccessor" />
    /// </summary>
    /// <seealso cref="NetSparkleUpdater.Interfaces.IAssemblyAccessor" />
    public class EntryAssemblyAccessor : IAssemblyAccessor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryAssemblyAccessor"/> class.
        /// </summary>
        public EntryAssemblyAccessor()
        {
            var info = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
            AssemblyVersion = info.FileVersion;
            AssemblyTitle = info.ProductName;
            AssemblyCompany = info.CompanyName;
            AssemblyCopyright = info.LegalCopyright;
            AssemblyProduct = info.ProductName;
            AssemblyDescription = info.FileDescription;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the assembly company.
        /// </summary>
        /// <value>The assembly company.</value>
        public string AssemblyCompany { get; private set; }

        /// <summary>
        /// Gets the assembly copyright.
        /// </summary>
        /// <value>The assembly copyright.</value>
        public string AssemblyCopyright { get; private set; }

        /// <summary>
        /// Gets the assembly description.
        /// </summary>
        /// <value>The assembly description.</value>
        public string AssemblyDescription { get; private set; }

        /// <summary>
        /// Gets the assembly product.
        /// </summary>
        /// <value>The assembly product.</value>
        public string AssemblyProduct { get; private set; }

        /// <summary>
        /// Gets the assembly title.
        /// </summary>
        /// <value>The assembly title.</value>
        public string AssemblyTitle { get; private set; }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        /// <value>The assembly version.</value>
        public string AssemblyVersion { get; private set; }

        #endregion Properties
    }
}
