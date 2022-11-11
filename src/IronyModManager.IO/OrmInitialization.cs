// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 11-04-2022
// ***********************************************************************
// <copyright file="OrmInitialization.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared;
using RepoDb;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class Initialization.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    [ExcludeFromCoverage("Setup module.")]
    public class OrmInitialization : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            // Initialize ORM
            GlobalConfiguration.Setup().UseSqlite();
            // Doesn't work as well with MDS which I prefer to use... Reported to the authors who said it should be turned on if you use MDS.
            GlobalConfiguration.Options.ConversionType = RepoDb.Enumerations.ConversionType.Automatic;
        }

        #endregion Methods
    }
}
