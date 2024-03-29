﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-13-2020
//
// Last Modified By : Mario
// Last Modified On : 02-11-2022
// ***********************************************************************
// <copyright file="MinMaxNumericUpDown.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Utilities;
using IronyModManager.Common;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class MinMaxNumericUpDown.
    /// Implements the <see cref="Avalonia.Controls.NumericUpDown" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.NumericUpDown" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class MinMaxNumericUpDown : NumericUpDown, IStyleable
    {
        #region Fields

        /// <summary>
        /// The minimum maximum allow spin property
        /// </summary>
        public static readonly StyledProperty<bool> MinMaxAllowSpinProperty = AvaloniaProperty.Register<MinMaxButtonSpinner, bool>(nameof(MinMaxAllowSpin), true);

        /// <summary>
        /// The minimum maximum button spinner location property
        /// </summary>
        public static readonly StyledProperty<Location> MinMaxButtonSpinnerLocationProperty = AvaloniaProperty.Register<MinMaxButtonSpinner, Location>(nameof(MinMaxButtonSpinnerLocation), Location.Left);

        /// <summary>
        /// Defines the <see cref="ShowButtonSpinner" /> property.
        /// </summary>
        public static readonly StyledProperty<bool> MinMaxShowButtonSpinnerProperty = AvaloniaProperty.Register<MinMaxButtonSpinner, bool>(nameof(MinMaxShowButtonSpinner), true);

        /// <summary>
        /// The delay
        /// </summary>
        private const int Delay = 50;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// The spinner
        /// </summary>
        private Spinner minMaxSpinner;

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
            AllowSpin = false;
            Initialized += (sender, e) =>
            {
                SetValidSpinDirection();
            };
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [spinned].
        /// </summary>
        public new event EventHandler<SpinEventArgs> Spinned;

        #endregion Events

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
        /// Handles the <see cref="E:TemplateApplied" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            // Would be kinda great if avalonia guys exposed some of these private properties. Not very customizable with this...
            if (minMaxSpinner != null)
            {
                minMaxSpinner.Spin -= OnMinMaxSpinnerSpin;
            }
            minMaxSpinner = e.NameScope.Find<Spinner>("PART_MinMax_Spinner");
            if (minMaxSpinner != null)
            {
                minMaxSpinner.Spin += OnMinMaxSpinnerSpin;
            }

            if (spinner != null)
            {
                spinner.Spin -= OnSpinnerSpin;
            }
            spinner = e.NameScope.Find<Spinner>("PART_Spinner");
            if (spinner != null)
            {
                spinner.Spin += OnSpinnerSpin;
            }

            textBox = e.NameScope.Find<TextBox>("PART_TextBox");

            SetValidSpinDirection();
        }

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
        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    base.OnKeyDown(e);
                    if (e.Handled)
                    {
                        SetValidSpinDirection();
                    }
                    break;

                default:
                    e.Handled = false;
                    break;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:LostFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        /// <inheritdoc />
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
        /// Called when [text changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnTextChanged(string oldValue, string newValue)
        {
            if (!(textBox != null && textBox.IsFocused))
            {
                base.OnTextChanged(oldValue, newValue);
                if (IsInitialized)
                {
                    SetValidSpinDirection();
                }
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
        /// do decrement as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task DoDecrementAsync(CancellationToken cancellationToken)
        {
            if (spinner == null || (spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
            {
                await Task.Delay(Delay, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    var result = Value - Increment;
                    await Dispatcher.UIThread.SafeInvokeAsync(() =>
                    {
                        Value = MathUtilities.Clamp(result, Minimum, Maximum);
                    });
                }
            }
        }

        /// <summary>
        /// do increment as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task DoIncrementAsync(CancellationToken cancellationToken)
        {
            if (spinner == null || (spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
            {
                await Task.Delay(Delay, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    var result = Value + Increment;
                    await Dispatcher.UIThread.SafeInvokeAsync(() =>
                    {
                        Value = MathUtilities.Clamp(result, Minimum, Maximum);
                    });
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:MinMaxSpinnerSpin" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpinEventArgs" /> instance containing the event data.</param>
        private void OnMinMaxSpinnerSpin(object sender, SpinEventArgs e)
        {
            if (MinMaxAllowSpin && !IsReadOnly)
            {
                var spin = !e.UsingMouseWheel;
                if (spin)
                {
                    e.Handled = true;
                    if (e.Direction == SpinDirection.Increase)
                    {
                        if (minMaxSpinner == null || (minMaxSpinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
                        {
                            Value = Maximum;
                        }
                    }
                    else
                    {
                        if (minMaxSpinner == null || (minMaxSpinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
                        {
                            Value = Minimum;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:SpinnerSpin" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SpinEventArgs" /> instance containing the event data.</param>
        private void OnSpinnerSpin(object sender, SpinEventArgs e)
        {
            if (!AllowSpin && MinMaxAllowSpin && !IsReadOnly)
            {
                var spin = !e.UsingMouseWheel;

                if (spin)
                {
                    e.Handled = true;
                    var handler = Spinned;
                    handler?.Invoke(this, e);

                    if (cancellationTokenSource != null)
                    {
                        cancellationTokenSource.Cancel();
                    }
                    cancellationTokenSource = new CancellationTokenSource();
                    if (e.Direction == SpinDirection.Increase)
                    {
                        DoIncrementAsync(cancellationTokenSource.Token).ConfigureAwait(true);
                    }
                    else
                    {
                        DoDecrementAsync(cancellationTokenSource.Token).ConfigureAwait(true);
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

            if (minMaxSpinner != null)
            {
                minMaxSpinner.ValidSpinDirection = validDirections;
            }
        }

        #endregion Methods
    }
}
