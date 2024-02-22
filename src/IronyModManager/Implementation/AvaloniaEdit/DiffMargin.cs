// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-19-2024
//
// Last Modified By : Mario
// Last Modified On : 02-22-2024
// ***********************************************************************
// <copyright file="DiffMargin.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Portions based on LineNumberingMargin</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;
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
        /// The maximum line number length
        /// </summary>
        private int maxLineNumberLength = 1;

        /// <summary>
        /// A private const double named LineMargin.
        /// </summary>
        private const double LineMargin = 4d;

        /// <summary>
        /// The font family
        /// </summary>
        private FontFamily fontFamily;

        /// <summary>
        /// The font size
        /// </summary>
        private double fontSize;

        /// <summary>
        /// The selecting
        /// </summary>
        private bool selecting;

        /// <summary>
        /// The selection start
        /// </summary>
        private AnchorSegment selectionStart;

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
                fontFamily = GetValue(TextBlock.FontFamilyProperty);
                fontSize = GetValue(TextBlock.FontSizeProperty);

                var txt = TextFormatterFactory.CreateFormattedText(this, new string('9', 2), fontFamily, fontSize, GetValue(TemplatedControl.ForegroundProperty));
                return new Size(txt.Bounds.Width + (LineMargin * 2), 0);
            }

            var text = Lines.LastOrDefault()!.Index.ToString();
            var typeFace = CreateTypeface();
            var lineText = new FormattedText(text, typeFace, TextView.GetValue(TextBlock.FontSizeProperty), TextAlignment.Left, TextWrapping.NoWrap, Size.Empty);
            return new Size(lineText.Bounds.Width + (LineMargin * 2), 0);
        }

        /// <summary>
        /// Called when [document changed].
        /// </summary>
        /// <param name="oldDocument">The old document.</param>
        /// <param name="newDocument">The new document.</param>
        protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
        {
            if (oldDocument != null)
            {
                TextDocumentWeakEventManager.LineCountChanged.RemoveHandler(oldDocument, OnDocumentLineCountChanged);
            }

            base.OnDocumentChanged(oldDocument, newDocument);
            if (newDocument != null)
            {
                TextDocumentWeakEventManager.LineCountChanged.AddHandler(newDocument, OnDocumentLineCountChanged);
            }

            OnDocumentLineCountChanged();
        }

        /// <summary>
        /// Called before the <see cref="E:Avalonia.Input.InputElement.PointerMoved" /> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (selecting && TextArea != null && TextView != null)
            {
                e.Handled = true;
                var currentSeg = GetTextLineSegment(e);
                if (currentSeg == SimpleSegment.Invalid)
                {
                    return;
                }

                ExtendSelection(currentSeg);
                TextArea.Caret.BringCaretToView(5.0);
            }

            base.OnPointerMoved(e);
        }

        /// <summary>
        /// Called before the <see cref="E:Avalonia.Input.InputElement.PointerPressed" /> event occurs.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (!e.Handled && TextView != null && TextArea != null)
            {
                e.Handled = true;
                TextArea.Focus();

                var currentSeg = GetTextLineSegment(e);
                if (currentSeg == SimpleSegment.Invalid)
                {
                    return;
                }

                TextArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
                e.Pointer.Capture(this);
                if (Equals(e.Pointer.Captured, this))
                {
                    selecting = true;
                    selectionStart = new AnchorSegment(Document, currentSeg.Offset, currentSeg.Length);
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        if (TextArea.Selection is SimpleSelection simpleSelection)
                            selectionStart = new AnchorSegment(Document, simpleSelection.SurroundingSegment);
                    }

                    TextArea.Selection = Selection.Create(TextArea, selectionStart);
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        ExtendSelection(currentSeg);
                    }

                    TextArea.Caret.BringCaretToView(5.0);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerReleased" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerReleasedEventArgs"/> instance containing the event data.</param>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (selecting)
            {
                selecting = false;
                selectionStart = null;
                e.Pointer.Capture(null);
                e.Handled = true;
            }

            base.OnPointerReleased(e);
        }

        /// <summary>
        /// Called when [text view changed].
        /// </summary>
        /// <param name="oldTextView">The old text view.</param>
        /// <param name="newTextView">The new text view.</param>
        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
            }

            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
            }

            InvalidateVisual();
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

        /// <summary>
        /// Extends the selection.
        /// </summary>
        /// <param name="currentSeg">The current seg.</param>
        private void ExtendSelection(SimpleSegment currentSeg)
        {
            if (currentSeg.Offset < selectionStart.Offset)
            {
                TextArea.Caret.Offset = currentSeg.Offset;
                TextArea.Selection = Selection.Create(TextArea, currentSeg.Offset, selectionStart.Offset + selectionStart.Length);
            }
            else
            {
                TextArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
                TextArea.Selection = Selection.Create(TextArea, selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
            }
        }

        /// <summary>
        /// Gets the text line segment.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event data.</param>
        /// <returns>SimpleSegment.</returns>
        private SimpleSegment GetTextLineSegment(PointerEventArgs e)
        {
            var pos = e.GetPosition(TextView);
            pos = new Point(0, pos.Y.CoerceValue(0, TextView.Bounds.Height) + TextView.VerticalOffset);
            var vl = TextView.GetVisualLineFromVisualTop(pos.Y);
            if (vl == null)
                return SimpleSegment.Invalid;
            var tl = vl.GetTextLineByVisualYPosition(pos.Y);
            var visualStartColumn = vl.GetTextLineVisualStartColumn(tl);
            var visualEndColumn = visualStartColumn + tl.Length;
            var relStart = vl.FirstDocumentLine.Offset;
            var startOffset = vl.GetRelativeOffset(visualStartColumn) + relStart;
            var endOffset = vl.GetRelativeOffset(visualEndColumn) + relStart;
            if (endOffset == vl.LastDocumentLine.Offset + vl.LastDocumentLine.Length)
                endOffset += vl.LastDocumentLine.DelimiterLength;
            return new SimpleSegment(startOffset, endOffset - startOffset);
        }

        /// <summary>
        /// Handles the <see cref="E:DocumentLineCountChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnDocumentLineCountChanged(object sender, EventArgs e)
        {
            OnDocumentLineCountChanged();
        }

        /// <summary>
        /// Called when [document line count changed].
        /// </summary>
        private void OnDocumentLineCountChanged()
        {
            var documentLineCount = Document?.LineCount ?? 1;
            var newLength = documentLineCount.ToString(CultureInfo.CurrentCulture).Length;

            if (newLength < 2)
            {
                newLength = 2;
            }

            if (newLength != maxLineNumberLength)
            {
                maxLineNumberLength = newLength;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Texts the view visual lines changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }

        #endregion Methods
    }
}
