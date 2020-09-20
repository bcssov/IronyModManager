// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="ZipExtractionOpts.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using SharpCompress.Common;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class ZipExtractionOpts.
    /// </summary>
    internal static class ZipExtractionOpts
    {
        #region Fields

        /// <summary>
        /// The extraction options
        /// </summary>
        private static ExtractionOptions extractionOptions;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the extraction options.
        /// </summary>
        /// <returns>ExtractionOptions.</returns>
        public static ExtractionOptions GetExtractionOptions()
        {
            if (extractionOptions == null)
            {
                extractionOptions = new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true,
                    PreserveFileTime = true
                };
            }
            return extractionOptions;
        }

        #endregion Methods
    }
}
