// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 03-13-2021
// ***********************************************************************
// <copyright file="NotoSansFontFamily.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Platform.Fonts;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Class NotoSansFontFamily.
    /// Implements the <see cref="IronyModManager.Platform.Fonts.BaseFontFamily" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Fonts.BaseFontFamily" />
    public class NotoSansFontFamily : BaseFontFamily
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotoSansFontFamily" /> class.
        /// </summary>
        public NotoSansFontFamily()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => Constants.DefaultFont;

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <value>The folder.</value>
        protected override string Folder => "NotoSans";

        #endregion Properties
    }
}
