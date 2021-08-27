// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 08-27-2021
//
// Last Modified By : Mario
// Last Modified On : 08-27-2021
// ***********************************************************************
// <copyright file="SearchSelect.cs" company="Mario">
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
using Avalonia.Threading;
using IronyModManager.Shared.Models;

namespace IronyModManager.Controls.Helper
{
    /// <summary>
    /// Class SearchSelect.
    /// </summary>
    public class SearchSelect
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

        #region Methods

        /// <summary>
        /// Finds the match.
        /// </summary>
        /// <param name="e">The <see cref="TextInputEventArgs" /> instance containing the event data.</param>
        /// <param name="isTextSearchEnabled">if set to <c>true</c> [is text search enabled].</param>
        /// <param name="itemContainerGenerator">The item container generator.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public int? FindMatch(TextInputEventArgs e, bool isTextSearchEnabled, IItemContainerGenerator itemContainerGenerator, IEnumerable<IQueryableModel> dataSource)
        {
            int? index = null;
            if (!e.Handled && isTextSearchEnabled)
            {
                StopTextSearchTimer();

                textSearchTerm += e.Text;

                bool matchContainer(ItemContainerInfo info)
                {
                    // Bad Avalonia implementation
                    if (info.ContainerControl is IContentControl control)
                    {
                        if (control.Content is IQueryableModel model)
                        {
                            return matchModel(model);
                        }
                        else
                        {
                            return control.Content?.ToString()?.StartsWith(textSearchTerm, StringComparison.OrdinalIgnoreCase) == true;
                        }
                    }
                    return false;
                }
                bool matchModel(IQueryableModel model) => model.IsMatch(textSearchTerm);

                var fallBack = true;
                if (dataSource != null && dataSource.Any())
                {
                    var col = dataSource.ToList();
                    var item = col.FirstOrDefault(matchModel);
                    if (item != null)
                    {
                        fallBack = false;
                        index = col.IndexOf(item);
                    }
                }
                if (fallBack)
                {
                    var info = itemContainerGenerator.Containers.FirstOrDefault(matchContainer);
                    if (info != null)
                    {
                        index = info.Index;
                    }
                }
                StartTextSearchTimer();

                e.Handled = true;
            }
            return index;
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
