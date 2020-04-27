// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-15-2020
//
// Last Modified By : Mario
// Last Modified On : 04-27-2020
// ***********************************************************************
// <copyright file="TextEditor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Avalonia.Styling;
using AvaloniaEdit.Editing;
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
            TextArea.TextCopied += (sender, args) =>
            {
                string text = string.Join(Environment.NewLine, args.Text.SplitOnNewLine());
                CopyTextAsync(text).ConfigureAwait(true);
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(AvaloniaEdit.TextEditor);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Copies the text asynchronous.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task.</returns>
        protected virtual Task CopyTextAsync(string text)
        {
            if (appAction == null)
            {
                appAction = DIResolver.Get<IAppAction>();
            }
            return appAction.CopyAsync(text);
        }

        #endregion Methods
    }
}
