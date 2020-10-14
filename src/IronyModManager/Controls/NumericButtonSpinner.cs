// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-14-2020
//
// Last Modified By : Mario
// Last Modified On : 10-14-2020
// ***********************************************************************
// <copyright file="NumericButtonSpinner.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Styling;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class NumericButtonSpinner.
    /// Implements the <see cref="Avalonia.Controls.ButtonSpinner" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ButtonSpinner" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class NumericButtonSpinner : ButtonSpinner, IStyleable
    {
        #region Fields

        /// <summary>
        /// The decrease button
        /// </summary>
        private Button decreaseButton;

        /// <summary>
        /// The increase button
        /// </summary>
        private Button increaseButton;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(ButtonSpinner);

        /// <summary>
        /// Gets or sets the DecreaseButton template part.
        /// </summary>
        /// <value>The decrease button.</value>
        private Button DecreaseButton
        {
            get
            {
                return decreaseButton;
            }
            set
            {
                if (decreaseButton != null)
                {
                    decreaseButton.Click -= OnButtonClick;
                }
                decreaseButton = value;
                if (decreaseButton != null)
                {
                    decreaseButton.Click += OnButtonClick;
                }
            }
        }

        /// <summary>
        /// Gets or sets the IncreaseButton template part.
        /// </summary>
        /// <value>The increase button.</value>
        private Button IncreaseButton
        {
            get
            {
                return increaseButton;
            }
            set
            {
                if (increaseButton != null)
                {
                    increaseButton.Click -= OnButtonClick;
                }
                increaseButton = value;
                if (increaseButton != null)
                {
                    increaseButton.Click += OnButtonClick;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [allow spin changed].
        /// </summary>
        /// <param name="oldValue">if set to <c>true</c> [old value].</param>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        protected override void OnAllowSpinChanged(bool oldValue, bool newValue)
        {
            base.OnAllowSpinChanged(oldValue, newValue);
            SetButtonUsage();
        }

        /// <summary>
        /// Handles the <see cref="E:TemplateApplied" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            IncreaseButton = e.NameScope.Find<Button>("PART_IncreaseButton");
            DecreaseButton = e.NameScope.Find<Button>("PART_DecreaseButton");
            SetButtonUsage();
        }

        /// <summary>
        /// Called when [valid spin direction changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            base.OnValidSpinDirectionChanged(oldValue, newValue);
            SetButtonUsage();
        }

        /// <summary>
        /// Handles the <see cref="E:ButtonClick" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var direction = sender == IncreaseButton ? SpinDirection.Increase : SpinDirection.Decrease;
            OnSpin(new SpinEventArgs(SpinEvent, direction));
        }

        /// <summary>
        /// Sets the button usage.
        /// </summary>
        private void SetButtonUsage()
        {
            if (IncreaseButton != null)
            {
                IncreaseButton.IsEnabled = (ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase;
            }

            if (DecreaseButton != null)
            {
                DecreaseButton.IsEnabled = (ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease;
            }
        }

        #endregion Methods
    }
}
