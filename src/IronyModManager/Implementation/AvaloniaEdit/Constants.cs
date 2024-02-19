// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-19-2024
//
// Last Modified By : Mario
// Last Modified On : 02-19-2024
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public class Constants
    {
        #region Fields

        /// <summary>
        /// A private static readonly SolidColorBrush named diffDeletedLine.
        /// </summary>
        public static readonly SolidColorBrush DiffDeletedLine = SolidColorBrush.Parse("#FF9999");

        /// <summary>
        /// A private static readonly SolidColorBrush named diffDeletedPieces.
        /// </summary>
        public static readonly SolidColorBrush DiffDeletedPieces = SolidColorBrush.Parse("#FF9999");

        /// <summary>
        /// A private static readonly SolidColorBrush named DiffImaginaryLine.
        /// </summary>
        public static readonly SolidColorBrush DiffImaginaryLine = SolidColorBrush.Parse("#FF808080");

        /// <summary>
        /// A private static readonly SolidColorBrush named diffInsertedLine.
        /// </summary>
        public static readonly SolidColorBrush DiffInsertedLine = SolidColorBrush.Parse("#66cc99");

        /// <summary>
        /// A private static readonly SolidColorBrush named diffInsertedPieces.
        /// </summary>
        public static readonly SolidColorBrush DiffInsertedPieces = SolidColorBrush.Parse("#66cc99");

        /// <summary>
        /// A private static readonly SolidColorBrush named diffModifiedPieces.
        /// </summary>
        public static readonly SolidColorBrush DiffModifiedPieces = SolidColorBrush.Parse("#ffe28b");

        /// <summary>
        /// A private static readonly SolidColorBrush named diffUnchangedPieces.
        /// </summary>
        public static readonly SolidColorBrush DiffUnchangedPieces = SolidColorBrush.Parse("#ffe28b");

        /// <summary>
        /// A public static readonly Pen named TransparentPen.
        /// </summary>
        public static readonly Pen TransparentPen = new(new SolidColorBrush(Brushes.Transparent.Color));

        #endregion Fields
    }
}
