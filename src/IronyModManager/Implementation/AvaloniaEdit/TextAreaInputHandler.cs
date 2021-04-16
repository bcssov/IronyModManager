// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-14-2020
//
// Last Modified By : Mario
// Last Modified On : 04-15-2021
// ***********************************************************************
// <copyright file="TextAreaInputHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.Editing;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// Class TextAreaInputHandler.
    /// Implements the <see cref="AvaloniaEdit.Editing.TextAreaDefaultInputHandler" />
    /// </summary>
    /// <seealso cref="AvaloniaEdit.Editing.TextAreaDefaultInputHandler" />
    public class TextAreaInputHandler : TextAreaDefaultInputHandler
    {
        #region Fields

        /// <summary>
        /// The editor
        /// </summary>
        private readonly TextEditor editor;

        /// <summary>
        /// The handle all events
        /// </summary>
        private bool handleAllEvents = false;

        /// <summary>
        /// The mouse handler
        /// </summary>
        private ITextAreaInputHandler mouseHandler;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextAreaInputHandler" /> class.
        /// </summary>
        /// <param name="editor">The editor.</param>
        public TextAreaInputHandler(TextEditor editor) : base(editor.TextArea)
        {
            this.editor = editor;
#pragma warning disable CS0618 // Type or member is obsolete
            editor.ContextMenu.MenuClosed += ContextMenu_MenuClosed;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Attaches this instance.
        /// </summary>
        public override void Attach()
        {
            TextArea.PointerPressed += TextArea_PointerPressed;
            TextArea.PointerMoved += TextArea_PointerMoved;
            TextArea.PointerReleased += TextArea_PointerReleased;
            base.Attach();
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public override void Detach()
        {
            TextArea.PointerPressed -= TextArea_PointerPressed;
            TextArea.PointerMoved -= TextArea_PointerMoved;
            TextArea.PointerReleased -= TextArea_PointerReleased;
            base.Detach();
        }

        /// <summary>
        /// reset flag as an asynchronous operation.
        /// </summary>
        protected async Task ResetFlagAsync()
        {
            await Task.Delay(100);
            handleAllEvents = false;
            if (mouseHandler != null)
            {
                mouseHandler.Detach();
                NestedInputHandlers.Add(mouseHandler);
            }
        }

        /// <summary>
        /// Handles the MenuClosed event of the ContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Avalonia.Interactivity.RoutedEventArgs" /> instance containing the event data.</param>
        private void ContextMenu_MenuClosed(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ResetFlagAsync().ConfigureAwait(true);
        }

        /// <summary>
        /// Determines whether [is left mouse button] [the specified p].
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns><c>true</c> if [is left mouse button] [the specified p]; otherwise, <c>false</c>.</returns>
        private bool IsRightMouseButton(PointerPoint p)
        {
            var updateType = p.Properties.PointerUpdateKind;
            return updateType == PointerUpdateKind.RightButtonPressed || updateType == PointerUpdateKind.RightButtonReleased;
        }

        /// <summary>
        /// Handles the PointerMoved event of the TextArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerEventArgs" /> instance containing the event data.</param>
        private void TextArea_PointerMoved(object sender, PointerEventArgs e)
        {
            // Avalonia edit is killing RMB
            e.Handled = handleAllEvents;
        }

        /// <summary>
        /// Handles the PointerPressed event of the TextArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        private void TextArea_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            // Avalonia edit is killing RMB
            var rmb = IsRightMouseButton(e.GetCurrentPoint(null));
#pragma warning disable CS0618 // Type or member is obsolete
            if (rmb && editor.ContextMenu != null)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                // Aside from forking the whole project over and fixing the mess, we can hack our way around. It just ain't worth it.
                if (mouseHandler == null)
                {
                    mouseHandler = NestedInputHandlers.FirstOrDefault(p => p.GetType().Name.Contains("SelectionMouseHandler"));
                }
                handleAllEvents = true;
                NestedInputHandlers.Remove(mouseHandler);
#pragma warning disable CS0618 // Type or member is obsolete
                editor.ContextMenu.Open(editor);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        /// <summary>
        /// Handles the PointerReleased event of the TextArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerReleasedEventArgs" /> instance containing the event data.</param>
        private void TextArea_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            // Avalonia edit is killing RMB
            e.Handled = handleAllEvents;
        }

        #endregion Methods
    }
}
