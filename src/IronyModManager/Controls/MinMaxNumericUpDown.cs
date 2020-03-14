// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-13-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2020
// ***********************************************************************
// <copyright file="MinMaxNumericUpDown.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class MinMaxNumericUpDown.
    /// Implements the <see cref="Avalonia.Controls.NumericUpDown" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.NumericUpDown" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class MinMaxNumericUpDown : NumericUpDown, IStyleable
    {
        #region Fields

        /// <summary>
        /// The minimum maximum allow spin property
        /// </summary>
        public static readonly StyledProperty<bool> MinMaxAllowSpinProperty = AvaloniaProperty.Register<ButtonSpinner, bool>(nameof(MinMaxAllowSpin), true);

        /// <summary>
        /// The minimum maximum button spinner location property
        /// </summary>
        public static readonly StyledProperty<Location> MinMaxButtonSpinnerLocationProperty = AvaloniaProperty.Register<ButtonSpinner, Location>(nameof(MinMaxButtonSpinnerLocation), Location.Left);

        /// <summary>
        /// Defines the <see cref="ShowButtonSpinner" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> MinMaxShowButtonSpinnerProperty = AvaloniaProperty.Register<ButtonSpinner, bool>(nameof(MinMaxShowButtonSpinner), true);

        /// <summary>
        /// The spinner
        /// </summary>
        private Spinner spinner;

        /// <summary>
        /// The text box
        /// </summary>
        private TextBox textBox;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxNumericUpDown" /> class.
        /// </summary>
        public MinMaxNumericUpDown() : base()
        {
            Initialized += (sender, e) =>
            {
                SetValidSpinDirection();
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [minimum maximum allow spin].
        /// </summary>
        /// <value><c>true</c> if [minimum maximum allow spin]; otherwise, <c>false</c>.</value>
        public bool MinMaxAllowSpin
        {
            get { return GetValue(MinMaxAllowSpinProperty); }
            set { SetValue(MinMaxAllowSpinProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum maximum button spinner location.
        /// </summary>
        /// <value>The minimum maximum button spinner location.</value>
        public Location MinMaxButtonSpinnerLocation
        {
            get { return GetValue(MinMaxButtonSpinnerLocationProperty); }
            set { SetValue(MinMaxButtonSpinnerLocationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [minimum maximum show button spinner].
        /// </summary>
        /// <value><c>true</c> if [minimum maximum show button spinner]; otherwise, <c>false</c>.</value>
        public bool MinMaxShowButtonSpinner
        {
            get { return GetValue(MinMaxShowButtonSpinnerProperty); }
            set { SetValue(MinMaxShowButtonSpinnerProperty, value); }
        }

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(MinMaxNumericUpDown);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [culture information changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            base.OnCultureInfoChanged(oldValue, newValue);
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Called when [format string changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnFormatStringChanged(string oldValue, string newValue)
        {
            base.OnFormatStringChanged(oldValue, newValue);
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Called when [increment changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnIncrementChanged(double oldValue, double newValue)
        {
            base.OnIncrementChanged(oldValue, newValue);
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Called when [is read only changed].
        /// </summary>
        /// <param name="oldValue">if set to <c>true</c> [old value].</param>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        protected override void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            base.OnIsReadOnlyChanged(oldValue, newValue);
            SetValidSpinDirection();
        }

        /// <summary>
        /// Handles the <see cref="E:KeyDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:LostFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            SetValidSpinDirection();
        }

        /// <summary>
        /// Called when [maximum changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnMaximumChanged(double oldValue, double newValue)
        {
            base.OnMaximumChanged(oldValue, newValue);
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Called when [minimum changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnMinimumChanged(double oldValue, double newValue)
        {
            base.OnMinimumChanged(oldValue, newValue);
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:TemplateApplied" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            // Would be kinda great if avalonia guys exposed some of these private properties. Not very customizable with this...
            if (spinner != null)
            {
                spinner.Spin -= OnSpinnerSpin;
            }

            spinner = e.NameScope.Find<Spinner>("PART_MinMax_Spinner");

            if (spinner != null)
            {
                spinner.Spin += OnSpinnerSpin;
            }

            textBox = e.NameScope.Find<TextBox>("PART_TextBox");

            SetValidSpinDirection();
        }

        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnTextChanged(string oldValue, string newValue)
        {
            base.OnTextChanged(oldValue, newValue);
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            SetValidSpinDirection();
        }

        /// <summary>
        /// Handles the <see cref="E:SpinnerSpin" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpinEventArgs" /> instance containing the event data.</param>
        private void OnSpinnerSpin(object sender, SpinEventArgs e)
        {
            if (MinMaxAllowSpin && !IsReadOnly)
            {
                var spin = !e.UsingMouseWheel;
                spin |= ((textBox != null) && textBox.IsFocused);

                if (spin)
                {
                    e.Handled = true;
                    if (e.Direction == SpinDirection.Increase)
                    {
                        if (spinner == null || (spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
                        {
                            Value = Maximum;
                        }
                    }
                    else
                    {
                        if (spinner == null || (spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
                        {
                            Value = Minimum;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the valid spin direction.
        /// </summary>
        private void SetValidSpinDirection()
        {
            var validDirections = ValidSpinDirections.None;

            if (Increment != 0 && !IsReadOnly)
            {
                if (Value < Maximum)
                {
                    validDirections |= ValidSpinDirections.Increase;
                }

                if (Value > Minimum)
                {
                    validDirections |= ValidSpinDirections.Decrease;
                }
            }

            if (spinner != null)
            {
                spinner.ValidSpinDirection = validDirections;
            }
        }

        #endregion Methods
    }
}
