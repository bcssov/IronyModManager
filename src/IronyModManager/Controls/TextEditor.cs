// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-15-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2024
// ***********************************************************************
// <copyright file="TextEditor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class TextEditor.
    /// Implements the <see cref="AvaloniaEdit.TextEditor" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    /// <seealso cref="AvaloniaEdit.TextEditor" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class TextEditor : AvaloniaEdit.TextEditor, IStyleable
    {
        #region Fields

        /// <summary>
        /// A private const double named MinimumDistanceToViewBorder.
        /// </summary>
        private const double MinimumDistanceToViewBorder = 30;

        /// <summary>
        /// The application action
        /// </summary>
        private IAppAction appAction;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditor" /> class.
        /// </summary>
        public TextEditor() : base(new TextArea())
        {
            TextArea.TextCopied += (_, args) =>
            {
                var text = string.Join(Environment.NewLine, args.Text.SplitOnNewLine());
                CopyTextAsync(text).ConfigureAwait(true);
            };
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [scroll initialized].
        /// </summary>
        public event EventHandler ScrollInitialized;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value representing the scroll viewer.<see cref="Avalonia.Controls.ScrollViewer" />
        /// </summary>
        /// <value>The scroll viewer.</value>
        public ScrollViewer ScrollViewer { get; private set; }

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(AvaloniaEdit.TextEditor);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Scroll to.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        public new void ScrollTo(int line, int column)
        {
            const double minimumScrollFraction = 0.3;
            ScrollTo(line, column, VisualYPosition.LineMiddle,
                null != ScrollViewer ? ScrollViewer.Viewport.Height / 2 : 0.0, minimumScrollFraction);
        }

        /// <summary>
        /// Scroll to.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <param name="yPositionMode">The y position mode.</param>
        /// <param name="referencedVerticalViewPortOffset">The referenced vertical view port offset.</param>
        /// <param name="minimumScrollFraction">The minimum scroll fraction.</param>
        public new void ScrollTo(int line, int column, VisualYPosition yPositionMode, double referencedVerticalViewPortOffset, double minimumScrollFraction)
        {
            // Backported from 0.11.x version
            var textView = TextArea.TextView;
            var document = textView.Document;
            if (ScrollViewer != null && document != null)
            {
                if (line < 1)
                    line = 1;
                if (line > document.LineCount)
                    line = document.LineCount;

                ILogicalScrollable scrollInfo = textView;
                if (!scrollInfo.CanHorizontallyScroll)
                {
                    var vl = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
                    var remainingHeight = referencedVerticalViewPortOffset;

                    while (remainingHeight > 0)
                    {
                        var prevLine = vl.FirstDocumentLine.PreviousLine;
                        if (prevLine == null)
                            break;
                        vl = textView.GetOrConstructVisualLine(prevLine);
                        remainingHeight -= vl.Height;
                    }
                }

                var p = TextArea.TextView.GetVisualPosition(
                    new TextViewPosition(line, Math.Max(1, column)),
                    yPositionMode);

                var targetX = ScrollViewer.Offset.X;
                var targetY = ScrollViewer.Offset.Y;

                var verticalPos = p.Y - referencedVerticalViewPortOffset;
                if (Math.Abs(verticalPos - ScrollViewer.Offset.Y) >
                    minimumScrollFraction * ScrollViewer.Viewport.Height)
                {
                    targetY = Math.Max(0, verticalPos);
                }

                if (column > 0)
                {
                    if (p.X > ScrollViewer.Viewport.Width - (MinimumDistanceToViewBorder * 2))
                    {
                        var horizontalPos = Math.Max(0, p.X - (ScrollViewer.Viewport.Width / 2));
                        if (Math.Abs(horizontalPos - ScrollViewer.Offset.X) >
                            minimumScrollFraction * ScrollViewer.Viewport.Width)
                        {
                            targetX = 0;
                        }
                    }
                    else
                    {
                        targetX = 0;
                    }
                }

                if (!targetX.IsNearlyEqual(ScrollViewer.Offset.X) || !targetY.IsNearlyEqual(ScrollViewer.Offset.Y))
                {
                    ScrollViewer.Offset = new Vector(targetX, targetY);
                }
            }
        }

        /// <summary>
        /// Scrolls to a line.
        /// </summary>
        /// <param name="line">The line.</param>
        public new void ScrollToLine(int line)
        {
            ScrollTo(line, -1);
        }

        /// <summary>
        /// Copies the text asynchronous.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task.</returns>
        protected virtual Task CopyTextAsync(string text)
        {
            appAction ??= DIResolver.Get<IAppAction>();
            return appAction.CopyAsync(text);
        }

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            // Yes, only to expose scroll viewer
            ScrollViewer = (ScrollViewer)e.NameScope.Find("PART_ScrollViewer");
            if (ScrollViewer != null)
            {
                ScrollInitialized?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}
