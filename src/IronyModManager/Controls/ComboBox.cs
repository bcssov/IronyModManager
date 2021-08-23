// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 08-23-2021
//
// Last Modified By : Mario
// Last Modified On : 08-23-2021
// ***********************************************************************
// <copyright file="ComboBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using IronyModManager.Shared.Models;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class ComboBox.
    /// Implements the <see cref="Avalonia.Controls.ComboBox" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ComboBox" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class ComboBox : Avalonia.Controls.ComboBox, IStyleable
    {
        #region Fields

        /// <summary>
        /// The text search term
        /// </summary>
        private string textSearchTerm = string.Empty;

        /// <summary>
        /// The text search timer
        /// </summary>
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private DispatcherTimer? textSearchTimer;

        #endregion Fields

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Avalonia.Controls.ComboBox);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:TextInput" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TextInputEventArgs" /> instance containing the event data.</param>
        protected override void OnTextInput(TextInputEventArgs e)
        {
            if (!e.Handled)
            {
                if (!IsTextSearchEnabled)
                    return;

                StopTextSearchTimer();

                textSearchTerm += e.Text;

                bool match(ItemContainerInfo info)
                {
                    // Bad Avalonia implementation
                    if (info.ContainerControl is IContentControl control)
                    {
                        if (control.Content is IQueryableModel model)
                        {
                            return model.IsMatch(textSearchTerm);
                        }
                        else
                        {
                            return control.Content?.ToString()?.StartsWith(textSearchTerm, StringComparison.OrdinalIgnoreCase) == true;
                        }
                    }
                    return false;
                }

                var info = ItemContainerGenerator.Containers.FirstOrDefault(match);

                if (info != null)
                {
                    SelectedIndex = info.Index;
                }

                StartTextSearchTimer();

                e.Handled = true;
            }
            base.OnTextInput(e);
        }

        /// <summary>
        /// Starts the text search timer.
        /// </summary>
        private void StartTextSearchTimer()
        {
            textSearchTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            textSearchTimer.Tick += TextSearchTimer_Tick;
            textSearchTimer.Start();
        }

        /// <summary>
        /// Stops the text search timer.
        /// </summary>
        private void StopTextSearchTimer()
        {
            if (textSearchTimer == null)
            {
                return;
            }

            textSearchTimer.Tick -= TextSearchTimer_Tick;
            textSearchTimer.Stop();

            textSearchTimer = null;
        }

        /// <summary>
        /// Handles the Tick event of the TextSearchTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TextSearchTimer_Tick(object sender, EventArgs e)
        {
            textSearchTerm = string.Empty;
            StopTextSearchTimer();
        }

        #endregion Methods
    }
}
