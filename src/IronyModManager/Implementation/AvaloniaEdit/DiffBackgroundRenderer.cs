// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-19-2024
//
// Last Modified By : Mario
// Last Modified On : 02-23-2024
// ***********************************************************************
// <copyright file="DiffBackgroundRenderer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using IronyModManager.DI;
using IronyModManager.Platform.Themes;
using IronyModManager.Services.Common;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// The diff background renderer.
    /// </summary>
    /// <seealso cref="IBackgroundRenderer" />
    public class DiffBackgroundRenderer : IBackgroundRenderer
    {
        #region Fields

        /// <summary>
        /// A private bool? named isLightTheme.
        /// </summary>
        private bool? isLightTheme;

        /// <summary>
        /// A private IThemeManager named themeManager.
        /// </summary>
        private IThemeManager themeManager;

        /// <summary>
        /// A private IThemeService named themeService.
        /// </summary>
        private IThemeService themeService;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a value representing the layer.<see cref="KnownLayer" />
        /// </summary>
        /// <value>The layer.</value>
        public KnownLayer Layer => KnownLayer.Background;

        /// <summary>
        /// Gets or sets a value representing the lines.<see cref="MergeViewerControlViewModel.DiffPieceWithIndex" />
        /// </summary>
        /// <value>The lines.</value>
        public IList<MergeViewerControlViewModel.DiffPieceWithIndex> Lines { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Draws the specified text view.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="drawingContext">The drawing context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (Lines == null || Lines.Count == 0)
            {
                return;
            }

            foreach (var line in textView.VisualLines)
            {
                var num = line.FirstDocumentLine.LineNumber - 1;
                if (num >= Lines.Count)
                {
                    continue;
                }

                var diff = Lines[num];

                Brush brush = diff.Type switch
                {
                    DiffPlex.DiffBuilder.Model.ChangeType.Deleted => IsLightTheme() ? Constants.LightDiffDeletedLine : Constants.DarkDiffDeletedLine,
                    DiffPlex.DiffBuilder.Model.ChangeType.Inserted => IsLightTheme() ? Constants.LightDiffInsertedLine : Constants.DarkDiffInsertedLine,
                    DiffPlex.DiffBuilder.Model.ChangeType.Imaginary => IsLightTheme() ? Constants.LightDiffImaginaryLine : Constants.DarkDiffImaginaryLine,
                    _ => default
                };

                if (brush != default(Brush))
                {
                    var rect = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, line, 0, 1000);
                    foreach (var r in rect)
                    {
                        drawingContext.DrawRectangle(brush, Constants.TransparentPen, new Rect(0, r.Top, textView.Bounds.Width, r.Height));
                    }
                }

                var offset = 0;
                var endOffset = 0;
                foreach (var piece in diff.SubPieces)
                {
                    var subPieceBrush = piece.Type switch
                    {
                        DiffPlex.DiffBuilder.Model.ChangeType.Deleted => IsLightTheme() ? Constants.LightDiffDeletedPieces : Constants.DarkDiffDeletedPieces,
                        DiffPlex.DiffBuilder.Model.ChangeType.Inserted => IsLightTheme() ? Constants.LightDiffInsertedPieces : Constants.DarkDiffInsertedPieces,
                        DiffPlex.DiffBuilder.Model.ChangeType.Modified => IsLightTheme() ? Constants.LightDiffModifiedPieces : Constants.DarkDiffModifiedPieces,
                        DiffPlex.DiffBuilder.Model.ChangeType.Unchanged => IsLightTheme() ? Constants.LightDiffUnchangedPieces : Constants.DarkDiffUnchangedPieces,
                        _ => default(Brush)
                    };
                    if (subPieceBrush != default(Brush))
                    {
                        var builder = new BackgroundGeometryBuilder { AlignToWholePixels = true };
                        endOffset += piece.Text.Length;
                        var diffSegment = new DiffSegment(line.StartOffset + offset, piece.Text.Length, line.StartOffset + endOffset);
                        offset = piece.Text.Length;
                        builder.AddSegment(textView, diffSegment);

                        var geo = builder.CreateGeometry();
                        if (geo != null)
                        {
                            drawingContext.DrawGeometry(brush, null, geo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is light theme.
        /// </summary>
        /// <returns>A bool.</returns>
        private bool IsLightTheme()
        {
            if (!isLightTheme.HasValue)
            {
                themeManager ??= DIResolver.Get<IThemeManager>();
                themeService ??= DIResolver.Get<IThemeService>();
                isLightTheme = themeManager.IsLightTheme(themeService.GetSelected().Type);
            }

            return isLightTheme.GetValueOrDefault();
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// The diff segment.
        /// </summary>
        /// <seealso cref="ISegment" />
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="endOffset">The end offset.</param>
        /// <remarks>Initializes a new instance of the <see cref="DiffSegment" /> class.</remarks>
        private class DiffSegment(int offset, int length, int endOffset) : ISegment
        {
            #region Properties

            /// <summary>
            /// Gets a value representing the end offset.
            /// </summary>
            /// <value>The end offset.</value>
            public int EndOffset { get; } = endOffset;

            /// <summary>
            /// Gets a value representing the length.
            /// </summary>
            /// <value>The length.</value>
            public int Length { get; } = length;

            /// <summary>
            /// Gets a value representing the offset.
            /// </summary>
            /// <value>The offset.</value>
            public int Offset { get; } = offset;

            #endregion Properties
        }

        #endregion Classes
    }
}
