// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-19-2024
//
// Last Modified By : Mario
// Last Modified On : 02-28-2024
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
        /// A public static readonly SolidColorBrush named DarkDiffDeletedLine.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffDeletedLine = SolidColorBrush.Parse("#923E3E");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffDeletedPieces.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffDeletedPieces = SolidColorBrush.Parse("#923E3E");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffImaginaryLine.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffImaginaryLine = SolidColorBrush.Parse("#5A5A5A");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffInsertedLine.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffInsertedLine = SolidColorBrush.Parse("#1B4F35");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffInsertedPieces.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffInsertedPieces = SolidColorBrush.Parse("#1B4F35");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffModifiedLine.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffModifiedLine = SolidColorBrush.Parse("#C49400");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffModifiedPieces.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffModifiedPieces = SolidColorBrush.Parse("#C49400");

        /// <summary>
        /// A public static readonly SolidColorBrush named DarkDiffUnchangedPieces.
        /// </summary>
        public static readonly SolidColorBrush DarkDiffUnchangedPieces = SolidColorBrush.Parse("#C49400");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffDeletedLine.
        /// </summary>
        public static readonly SolidColorBrush LightDiffDeletedLine = SolidColorBrush.Parse("#FF9999");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffDeletedPieces.
        /// </summary>
        public static readonly SolidColorBrush LightDiffDeletedPieces = SolidColorBrush.Parse("#FF9999");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffImaginaryLine.
        /// </summary>
        public static readonly SolidColorBrush LightDiffImaginaryLine = SolidColorBrush.Parse("#FF808080");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffInsertedLine.
        /// </summary>
        public static readonly SolidColorBrush LightDiffInsertedLine = SolidColorBrush.Parse("#66cc99");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffInsertedPieces.
        /// </summary>
        public static readonly SolidColorBrush LightDiffInsertedPieces = SolidColorBrush.Parse("#66cc99");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffModifiedLine.
        /// </summary>
        public static readonly SolidColorBrush LightDiffModifiedLine = SolidColorBrush.Parse("#ffe28b");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffModifiedPieces.
        /// </summary>
        public static readonly SolidColorBrush LightDiffModifiedPieces = SolidColorBrush.Parse("#ffe28b");

        /// <summary>
        /// A public static readonly SolidColorBrush named LightDiffUnchangedPieces.
        /// </summary>
        public static readonly SolidColorBrush LightDiffUnchangedPieces = SolidColorBrush.Parse("#ffe28b");

        /// <summary>
        /// A public static readonly Pen named TransparentPen.
        /// </summary>
        public static readonly Pen TransparentPen = new(new SolidColorBrush(Brushes.Transparent.Color));

        #endregion Fields
    }
}
