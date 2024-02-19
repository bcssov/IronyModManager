// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-19-2024
//
// Last Modified By : Mario
// Last Modified On : 02-19-2024
// ***********************************************************************
// <copyright file="DiffMargin.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// The diff margin.
    /// </summary>
    /// <seealso cref="AvaloniaEdit.Editing.AbstractMargin" />
    public class DiffMargin : AbstractMargin
    {
        #region Fields

        /// <summary>
        /// A private const double named LineMargin.
        /// </summary>
        private const double LineMargin = 4d;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value representing the lines.<see cref="System.Collections.Generic.IList{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel.DiffPieceWithIndex}" />
        /// </summary>
        /// <value>The lines.</value>
        public IList<MergeViewerControlViewModel.DiffPieceWithIndex> Lines { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Render.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (Lines == null || Lines.Count == 0)
            {
                return;
            }

            var typeFace = CreateTypeface();

            var visualLines = TextView.VisualLinesValid ? TextView.VisualLines : Enumerable.Empty<VisualLine>();
            foreach (var line in visualLines)
            {
                var rect = BackgroundGeometryBuilder.GetRectsFromVisualSegment(TextView, line, 0, 1000);
                var ln = line.FirstDocumentLine.LineNumber - 1;
                if (ln >= Lines.Count)
                {
                    continue;
                }

                var diff = Lines[ln];

                Brush brush = diff.Type switch
                {
                    DiffPlex.DiffBuilder.Model.ChangeType.Deleted => Constants.DiffDeletedLine,
                    DiffPlex.DiffBuilder.Model.ChangeType.Inserted => Constants.DiffInsertedLine,
                    DiffPlex.DiffBuilder.Model.ChangeType.Imaginary => Constants.DiffImaginaryLine,
                    _ => default
                };

                foreach (var r in rect)
                {
                    context.DrawRectangle(brush, Constants.TransparentPen, new Rect(0, r.Top, Bounds.Width, r.Height));
                }

                var text = new FormattedText(diff.Index.ToString(), typeFace, TextView.GetValue(TextBlock.FontSizeProperty), TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
                context.DrawText(TextView.GetValue(TextBlock.ForegroundProperty), new Point(LineMargin, rect.FirstOrDefault().Top), text);
            }
        }

        /// <summary>
        /// Measure override.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A Size.<see cref="Size" /></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Lines == null || Lines.Count == 0)
            {
                return new Size(0, 0);
            }

            var text = Lines.LastOrDefault()!.Index.ToString();
            var typeFace = CreateTypeface();
            var lineText = new FormattedText(text, typeFace, TextView.GetValue(TextBlock.FontSizeProperty), TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
            return new Size(lineText.Bounds.Width + LineMargin * 2, 0);
        }

        /// <summary>
        /// Create typeface.
        /// </summary>
        /// <returns>A Typeface.<see cref="Typeface" /></returns>
        private Typeface CreateTypeface()
        {
            return new Typeface(TextView.GetValue(TextBlock.FontFamilyProperty),
                TextView.GetValue(TextBlock.FontStyleProperty),
                TextView.GetValue(TextBlock.FontWeightProperty));
        }

        #endregion Methods
    }
}
