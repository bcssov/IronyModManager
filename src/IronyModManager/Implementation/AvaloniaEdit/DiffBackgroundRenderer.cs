// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-19-2024
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
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
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// The diff background renderer.
    /// </summary>
    /// <seealso cref="IBackgroundRenderer" />
    public class DiffBackgroundRenderer : IBackgroundRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value representing the color converter.<see cref="IronyModManager.Implementation.AvaloniaEdit.EditorColorConverter" />
        /// </summary>
        /// <value>The color converter.</value>
        public EditorColorConverter ColorConverter { get; set; }

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
                ColorConverter ??= new EditorColorConverter(null);

                Brush brush = diff.Type switch
                {
                    DiffPlex.DiffBuilder.Model.ChangeType.Deleted => ColorConverter.GetDeletedLineBrush(),
                    DiffPlex.DiffBuilder.Model.ChangeType.Inserted => ColorConverter.GetInsertedLineBrush(),
                    DiffPlex.DiffBuilder.Model.ChangeType.Imaginary => ColorConverter.GetImaginaryLineBrush(),
                    DiffPlex.DiffBuilder.Model.ChangeType.Modified => ColorConverter.GetEditedLineBrush(),
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
                        DiffPlex.DiffBuilder.Model.ChangeType.Deleted => ColorConverter.GetDeletedLineBrush(),
                        DiffPlex.DiffBuilder.Model.ChangeType.Inserted => ColorConverter.GetInsertedLineBrush(),
                        DiffPlex.DiffBuilder.Model.ChangeType.Modified => ColorConverter.GetEditedLineBrush(),
                        DiffPlex.DiffBuilder.Model.ChangeType.Unchanged => ColorConverter.GetEditedLineBrush(),
                        _ => default(Brush)
                    };
                    var text = piece.Text ?? string.Empty;
                    endOffset += text.Length;
                    if (subPieceBrush != default(Brush))
                    {
                        var builder = new BackgroundGeometryBuilder { AlignToWholePixels = true };
                        var diffSegment = new DiffSegment(line.StartOffset + offset, text.Length, line.StartOffset + endOffset);
                        builder.AddSegment(textView, diffSegment);

                        var geo = builder.CreateGeometry();
                        if (geo != null)
                        {
                            drawingContext.DrawGeometry(subPieceBrush, null, geo);
                        }
                    }

                    offset += text.Length;
                }
            }
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
