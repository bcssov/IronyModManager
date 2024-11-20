// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-20-2024
// ***********************************************************************
// <copyright file="ThemeCaptionButton.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Avalonia.Styling;

namespace IronyModManager.Controls.ClientSideDecorations
{
    /// <summary>
    /// Class ThemeCaptionButton.
    /// Implements the <see cref="Avalonia.Controls.Button" />
    /// Implements the <see cref="IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Button" />
    /// <seealso cref="IStyleable" />
    public class ThemeCaptionButton : Avalonia.Controls.Button, IStyleable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeCaptionButton"/> class.
        /// </summary>
        public ThemeCaptionButton() => Background = Brushes.Transparent;

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(ThemeCaptionButton);

        #endregion Properties
    }
}
