// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-01-2020
//
// Last Modified By : Mario
// Last Modified On : 10-13-2020
// ***********************************************************************
// <copyright file="SystemFontFamily.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Media;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Class SystemFontFamily.
    /// Implements the <see cref="IronyModManager.Fonts.IFontFamily" />
    /// </summary>
    /// <seealso cref="IronyModManager.Fonts.IFontFamily" />
    public class SystemFontFamily : IFontFamily
    {
        #region Fields

        /// <summary>
        /// The instance
        /// </summary>
        private FontFamily instance;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemFontFamily" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SystemFontFamily(string name)
        {
            Name = name;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the resource URI.
        /// </summary>
        /// <value>The resource URI.</value>
        public string ResourceUri => string.Empty;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <returns>FontFamily.</returns>
        public FontFamily GetFontFamily()
        {
            if (instance == null)
            {
                instance = new FontFamily(Name);
            }
            return instance;
        }

        #endregion Methods
    }
}
