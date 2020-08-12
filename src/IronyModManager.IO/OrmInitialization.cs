// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="OrmInitialization.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
            SqLiteBootstrap.Initialize();
            // Doesn't work as well with MDS which I prefer to use... Reported to the authors who said it should be turned on if you use MDS.
            Converter.ConversionType = RepoDb.Enumerations.ConversionType.Automatic;
        }

        #endregion Methods
    }
}
