﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 08-18-2022
//
// Last Modified By : Mario
// Last Modified On : 09-18-2022
// ***********************************************************************
// <copyright file="ScrollContentPresenter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Styling;
using IronyModManager.DI;
using IronyModManager.Implementation.AppState;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class ScrollContentPresenter.
    /// Implements the <see cref="Avalonia.Controls.Presenters.ScrollContentPresenter" />
    /// Implements the <see cref="IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Presenters.ScrollContentPresenter" />
    /// <seealso cref="IStyleable" />
    public class ScrollContentPresenter : Avalonia.Controls.Presenters.ScrollContentPresenter, IStyleable
    {
        #region Fields

        /// <summary>
        /// The scroll state
        /// </summary>
        private IScrollState scrollState;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Avalonia.Controls.Presenters.ScrollContentPresenter);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:PointerWheelChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerWheelEventArgs"/> instance containing the event data.</param>
        /// <inheritdoc />
        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            if (GetScrollState().GetCurrentState())
            {
                try
                {
                    base.OnPointerWheelChanged(e);
                }
                catch (IndexOutOfRangeException ex)
                {
                    // Unresolved issue in Avalonia, attempt not to crash
                    var logger = DIResolver.Get<ILogger>();
                    logger.Error(ex);
                    e.Handled = false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets the state of the scroll.
        /// </summary>
        /// <returns>IScrollState.</returns>
        private IScrollState GetScrollState()
        {
            if (scrollState == null)
            {
                scrollState = DIResolver.Get<IScrollState>();
            }
            return scrollState;
        }

        #endregion Methods
    }
}
