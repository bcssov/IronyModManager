// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-14-2020
//
// Last Modified By : Mario
// Last Modified On : 04-14-2020
// ***********************************************************************
// <copyright file="TextAreaInputHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
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

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextAreaInputHandler" /> class.
        /// </summary>
        /// <param name="editor">The editor.</param>
        public TextAreaInputHandler(TextEditor editor) : base(editor.TextArea)
        {
            this.editor = editor;
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
            e.Handled = (editor.ContextMenu?.IsOpen).GetValueOrDefault();
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
            if (rmb && editor.ContextMenu != null)
            {
                editor.ContextMenu.Open(editor);
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
            e.Handled = (editor.ContextMenu?.IsOpen).GetValueOrDefault();
        }

        #endregion Methods
    }
}
