// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-19-2024
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
// ***********************************************************************
// <copyright file="EditorColorConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Platform.Themes;
using IronyModManager.Services.Common;
using JetBrains.Annotations;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// Class EditorColorConverter.
    /// </summary>
    public class EditorColorConverter(IConflictSolverColors color)
    {
        #region Fields

        /// <summary>
        /// A private readonly IConflictSolverColors named color.
        /// </summary>
        private readonly IConflictSolverColors color = color;

        /// <summary>
        /// The deleted line brush
        /// </summary>
        [CanBeNull] private SolidColorBrush deletedLineBrush;

        /// <summary>
        /// The deleted line color
        /// </summary>
        private Color? deletedLineColor;

        /// <summary>
        /// The edited line brush
        /// </summary>
        [CanBeNull] private SolidColorBrush editedLineBrush;

        /// <summary>
        /// The edited line color
        /// </summary>
        private Color? editedLineColor;

        /// <summary>
        /// The imaginary line brush
        /// </summary>
        [CanBeNull] private SolidColorBrush imaginaryLineBrush;

        /// <summary>
        /// The imaginary line color
        /// </summary>
        private Color? imaginaryLineColor;

        /// <summary>
        /// The inserted line brush
        /// </summary>
        [CanBeNull] private SolidColorBrush insertedLineBrush;

        /// <summary>
        /// The inserted line clor
        /// </summary>
        private Color? insertedLineClor;

        /// <summary>
        /// The is light theme
        /// </summary>
        private bool? isLightTheme;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the default color of the deleted line.
        /// </summary>
        /// <returns>Color.</returns>
        public Color GetDefaultDeletedLineColor()
        {
            return IsLightTheme() ? Constants.LightDiffDeletedLine.Color : Constants.DarkDiffDeletedLine.Color;
        }

        /// <summary>
        /// Gets a default edited line color.
        /// </summary>
        /// <returns>A Color.<see cref="Color" /></returns>
        public Color GetDefaultEditedLineColor()
        {
            return IsLightTheme() ? Constants.LightDiffModifiedLine.Color : Constants.DarkDiffModifiedLine.Color;
        }

        /// <summary>
        /// Gets a default imaginary line color.
        /// </summary>
        /// <returns>A Color.<see cref="Color" /></returns>
        public Color GetDefaultImaginaryLineColor()
        {
            return IsLightTheme() ? Constants.LightDiffImaginaryLine.Color : Constants.DarkDiffImaginaryLine.Color;
        }

        /// <summary>
        /// Gets a default inserted line color.
        /// </summary>
        /// <returns>A Color.<see cref="Color" /></returns>
        public Color GetDefaultInsertedLineColor()
        {
            return IsLightTheme() ? Constants.LightDiffInsertedLine.Color : Constants.DarkDiffInsertedLine.Color;
        }

        /// <summary>
        /// Gets a deleted line brush.
        /// </summary>
        /// <returns>A SolidColorBrush.<see cref="SolidColorBrush" /></returns>
        public SolidColorBrush GetDeletedLineBrush()
        {
            deletedLineBrush ??= ToBrush(GetDeletedLineColor());
            return deletedLineBrush;
        }

        /// <summary>
        /// Gets the color of the deleted line.
        /// </summary>
        /// <returns>Color.</returns>
        public Color GetDeletedLineColor()
        {
            Color getColor()
            {
                if (color != null)
                {
                    var result = ConvertToColor(color.ConflictSolverDeletedLineColor);
                    if (result.HasValue)
                    {
                        return result.GetValueOrDefault();
                    }
                }

                return IsLightTheme() ? Constants.LightDiffDeletedLine.Color : Constants.DarkDiffDeletedLine.Color;
            }

            deletedLineColor ??= getColor();
            return deletedLineColor.GetValueOrDefault();
        }

        /// <summary>
        /// Gets an edited line brush.
        /// </summary>
        /// <returns>A SolidColorBrush.<see cref="SolidColorBrush" /></returns>
        public SolidColorBrush GetEditedLineBrush()
        {
            editedLineBrush ??= ToBrush(GetEditedLineColor());
            return editedLineBrush;
        }

        /// <summary>
        /// Gets the color of the edited line.
        /// </summary>
        /// <returns>Color.</returns>
        public Color GetEditedLineColor()
        {
            Color getColor()
            {
                if (color != null)
                {
                    var result = ConvertToColor(color.ConflictSolverModifiedLineColor);
                    if (result.HasValue)
                    {
                        return result.GetValueOrDefault();
                    }
                }

                return IsLightTheme() ? Constants.LightDiffModifiedLine.Color : Constants.DarkDiffModifiedLine.Color;
            }

            editedLineColor ??= getColor();
            return editedLineColor.GetValueOrDefault();
        }

        /// <summary>
        /// Gets an imaginary line brush.
        /// </summary>
        /// <returns>A SolidColorBrush.<see cref="SolidColorBrush" /></returns>
        public SolidColorBrush GetImaginaryLineBrush()
        {
            imaginaryLineBrush ??= ToBrush(GetImaginaryLineColor());
            return imaginaryLineBrush;
        }

        /// <summary>
        /// Gets the color of the imaginary line.
        /// </summary>
        /// <returns>Color.</returns>
        public Color GetImaginaryLineColor()
        {
            Color getColor()
            {
                if (color != null)
                {
                    var result = ConvertToColor(color.ConflictSolverImaginaryLineColor);
                    if (result.HasValue)
                    {
                        return result.GetValueOrDefault();
                    }
                }

                return IsLightTheme() ? Constants.LightDiffImaginaryLine.Color : Constants.DarkDiffImaginaryLine.Color;
            }

            imaginaryLineColor ??= getColor();
            return imaginaryLineColor.GetValueOrDefault();
        }

        /// <summary>
        /// Gets an inserted line brush.
        /// </summary>
        /// <returns>A SolidColorBrush.<see cref="SolidColorBrush" /></returns>
        public SolidColorBrush GetInsertedLineBrush()
        {
            insertedLineBrush ??= ToBrush(GetInsertedLineColor());
            return insertedLineBrush;
        }

        /// <summary>
        /// Gets the color of the inserted line.
        /// </summary>
        /// <returns>Color.</returns>
        public Color GetInsertedLineColor()
        {
            Color getColor()
            {
                if (color != null)
                {
                    var result = ConvertToColor(color.ConflictSolverInsertedLineColor);
                    if (result.HasValue)
                    {
                        return result.GetValueOrDefault();
                    }
                }

                return IsLightTheme() ? Constants.LightDiffInsertedLine.Color : Constants.DarkDiffInsertedLine.Color;
            }

            insertedLineClor ??= getColor();
            return insertedLineClor.GetValueOrDefault();
        }

        /// <summary>
        /// Converts to color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>System.Nullable&lt;Color&gt;.</returns>
        private Color? ConvertToColor(string color)
        {
            if (Color.TryParse(color, out var result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Determines whether [is light theme].
        /// </summary>
        /// <returns><c>true</c> if [is light theme]; otherwise, <c>false</c>.</returns>
        private bool IsLightTheme()
        {
            if (!isLightTheme.HasValue)
            {
                var themeManager = DIResolver.Get<IThemeManager>();
                var themeService = DIResolver.Get<IThemeService>();
                isLightTheme = themeManager.IsLightTheme(themeService.GetSelected().Type);
            }

            return isLightTheme.GetValueOrDefault();
        }

        /// <summary>
        /// To brush.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>A SolidColorBrush.<see cref="SolidColorBrush" /></returns>
        private SolidColorBrush ToBrush(Color color)
        {
            return new SolidColorBrush(color);
        }

        #endregion Methods
    }
}
