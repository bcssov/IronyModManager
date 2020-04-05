// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="HorizontalItemsRepeater.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Styling;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class HorizontalItemsRepeater.
    /// Implements the <see cref="Avalonia.Controls.ItemsRepeater" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ItemsRepeater" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class HorizontalItemsRepeater : ItemsRepeater, IStyleable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalItemsRepeater" /> class.
        /// </summary>
        public HorizontalItemsRepeater()
        {
            Layout = new StackLayout() { Orientation = Orientation.Horizontal };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(ItemsRepeater);

        #endregion Properties
    }
}
