// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 11-02-2022
// ***********************************************************************
// <copyright file="UpdaterConfiguration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using NetSparkleUpdater.Configurations;
using NetSparkleUpdater.Interfaces;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Class UpdaterConfiguration.
    /// Implements the <see cref="NetSparkleUpdater.Configurations.Configuration" />
    /// </summary>
    /// <seealso cref="NetSparkleUpdater.Configurations.Configuration" />
    public class UpdaterConfiguration : Configuration
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdaterConfiguration" /> class.
        /// </summary>
        /// <param name="assemblyAccessor">The assembly accessor.</param>
        public UpdaterConfiguration(IAssemblyAccessor assemblyAccessor) : base(assemblyAccessor)
        {
            // We want to handle this all manually at app startup and on user request
            DidRunOnce = true;
            IsFirstRun = false;
            CheckForUpdate = false;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        public override void Reload()
        {
            LastCheckTime = DateTime.MinValue;
            LastConfigUpdate = DateTime.MinValue;
            LastVersionSkipped = string.Empty;
        }

        /// <summary>
        /// Sets the version to skip.
        /// </summary>
        /// <param name="version">The version.</param>
        public override void SetVersionToSkip(string version)
        {
            base.SetVersionToSkip(version ?? string.Empty);
        }

        #endregion Methods
    }
}
