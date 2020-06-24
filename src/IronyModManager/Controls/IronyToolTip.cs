// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 06-18-2020
//
// Last Modified By : Mario
// Last Modified On : 06-24-2020
// ***********************************************************************
// <copyright file="IronyToolTip.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using IronyModManager.DI;
using Microsoft.Extensions.Configuration;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class IronyToolTip.
    /// Implements the <see cref="Avalonia.Controls.ContentControl" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ContentControl" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class IronyToolTip : ContentControl, IStyleable
    {
        #region Fields

        /// <summary>
        /// The horizontal offset property
        /// </summary>
        public static readonly AttachedProperty<double> HorizontalOffsetProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, double>("HorizontalOffset");

        /// <summary>
        /// The is open property
        /// </summary>
        public static readonly AttachedProperty<bool> IsOpenProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, bool>("IsOpen");

        /// <summary>
        /// The placement property
        /// </summary>
        public static readonly AttachedProperty<PlacementMode> PlacementProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, PlacementMode>("Placement", defaultValue: PlacementMode.Pointer);

        /// <summary>
        /// The react to parent property
        /// </summary>
        public static readonly AttachedProperty<bool> ReactToParentProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, bool>("ReactToParent");

        /// <summary>
        /// The show delay property
        /// </summary>
        public static readonly AttachedProperty<int> ShowDelayProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, int>("ShowDelay", 400);

        /// <summary>
        /// The tip property
        /// </summary>
        public static readonly AttachedProperty<object> TipProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, object>("Tip");

        /// <summary>
        /// The vertical offset property
        /// </summary>
        public static readonly AttachedProperty<double> VerticalOffsetProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, double>("VerticalOffset", 20);

        /// <summary>
        /// The tooltip section key
        /// </summary>
        private const string TooltipSectionKey = "Tooltips";

        /// <summary>
        /// The tooltip state key
        /// </summary>
        private const string TooltipStateKey = "Disable";

        /// <summary>
        /// The tool tip property
        /// </summary>
        private static readonly AttachedProperty<IronyToolTip> ToolTipProperty =
            AvaloniaProperty.RegisterAttached<IronyToolTip, Control, IronyToolTip>("ToolTip");

        /// <summary>
        /// The popup
        /// </summary>
        private IPopupHost _popup;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="IronyToolTip" /> class.
        /// </summary>
        static IronyToolTip()
        {
            TipProperty.Changed.Subscribe(ToolTipService.Instance.TipChanged);
            IsOpenProperty.Changed.Subscribe(IsOpenChanged);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(IronyToolTip);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the horizontal offset.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.Double.</returns>
        public static double GetHorizontalOffset(Control element)
        {
            return element.GetValue(HorizontalOffsetProperty);
        }

        /// <summary>
        /// Gets the is open.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetIsOpen(Control element)
        {
            return element.GetValue(IsOpenProperty);
        }

        /// <summary>
        /// Gets the placement.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>PlacementMode.</returns>
        public static PlacementMode GetPlacement(Control element)
        {
            return element.GetValue(PlacementProperty);
        }

        /// <summary>
        /// Gets the react to parent.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetReactToParent(Control element)
        {
            return element.GetValue(ReactToParentProperty);
        }

        /// <summary>
        /// Gets the show delay.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.Int32.</returns>
        public static int GetShowDelay(Control element)
        {
            return element.GetValue(ShowDelayProperty);
        }

        /// <summary>
        /// Gets the tip.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.Object.</returns>
        public static object GetTip(Control element)
        {
            return element.GetValue(TipProperty);
        }

        /// <summary>
        /// Gets the vertical offset.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.Double.</returns>
        public static double GetVerticalOffset(Control element)
        {
            return element.GetValue(VerticalOffsetProperty);
        }

        /// <summary>
        /// Sets the horizontal offset.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetHorizontalOffset(Control element, double value)
        {
            element.SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// Sets the is open.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsOpen(Control element, bool value)
        {
            element.SetValue(IsOpenProperty, value);
        }

        /// <summary>
        /// Sets the placement.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetPlacement(Control element, PlacementMode value)
        {
            element.SetValue(PlacementProperty, value);
        }

        /// <summary>
        /// Sets the react to parent.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetReactToParent(Control element, bool value)
        {
            element.SetValue(ReactToParentProperty, value);
        }

        /// <summary>
        /// Sets the show delay.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetShowDelay(Control element, int value)
        {
            element.SetValue(ShowDelayProperty, value);
        }

        /// <summary>
        /// Sets the tip.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetTip(Control element, object value)
        {
            element.SetValue(TipProperty, value);
        }

        /// <summary>
        /// Sets the vertical offset.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetVerticalOffset(Control element, double value)
        {
            element.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// Determines whether [is open changed] [the specified e].
        /// </summary>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void IsOpenChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var control = (Control)e.Sender;

            if ((bool)e.NewValue)
            {
                var tip = GetTip(control);
                if (tip == null) return;

                var toolTip = control.GetValue(ToolTipProperty);
                if (toolTip == null || (tip != toolTip && tip != toolTip.Content))
                {
                    toolTip?.Close();

                    toolTip = tip as IronyToolTip ?? new IronyToolTip { Content = tip };
                    control.SetValue(ToolTipProperty, toolTip);
                }

                toolTip.Open(control);
            }
            else
            {
                var toolTip = control.GetValue(ToolTipProperty);
                toolTip?.Close();
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        private void Close()
        {
            if (_popup != null)
            {
                _popup.SetChild(null);
                _popup.Dispose();
                _popup = null;
            }
        }

        /// <summary>
        /// Opens the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        private void Open(Control control)
        {
            if (!ToolTipService.TooltipsEnabled())
            {
                return;
            }
            Close();

            _popup = OverlayPopupHost.CreatePopupHost(control, null);
            _popup.SetChild(this);
            ((ISetLogicalParent)_popup).SetParent(control);

            _popup.ConfigurePosition(control, GetPlacement(control),
                new Point(GetHorizontalOffset(control), GetVerticalOffset(control)));
            _popup.Show();
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ToolTipService. This class cannot be inherited.
        /// </summary>
        private sealed class ToolTipService
        {
            #region Fields

            /// <summary>
            /// The tooltips enabled
            /// </summary>
            private static bool? tooltipsEnabled;

            /// <summary>
            /// The timer
            /// </summary>
            private DispatcherTimer _timer;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="ToolTipService" /> class from being created.
            /// </summary>
            private ToolTipService() { }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the instance.
            /// </summary>
            /// <value>The instance.</value>
            public static ToolTipService Instance { get; } = new ToolTipService();

            #endregion Properties

            #region Methods

            /// <summary>
            /// Tooltipses the enabled.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            internal static bool TooltipsEnabled()
            {
                if (!tooltipsEnabled.HasValue)
                {
                    var service = DIResolver.Get<IConfigurationRoot>();
                    var section = service.GetSection(TooltipSectionKey);
                    tooltipsEnabled = !section.GetSection(TooltipStateKey).Get<bool>();
                }
                return tooltipsEnabled.GetValueOrDefault();
            }

            /// <summary>
            /// called when the <see cref="ToolTip.TipProperty" /> property changes on a control.
            /// </summary>
            /// <param name="e">The event args.</param>
            internal void TipChanged(AvaloniaPropertyChangedEventArgs e)
            {
                if (!TooltipsEnabled())
                {
                    return;
                }
                var control = (Control)e.Sender;

                void parentControlLeave(object sender, PointerEventArgs args)
                {
                    HandleControlPointerLeave(control, true);
                }

                void parentControlEnter(object sender, PointerEventArgs args)
                {
                    HandleControlPointerEnter(control, true);
                }

                if (e.OldValue != null)
                {
                    control.PointerEnter -= ControlPointerEnter;
                    control.PointerLeave -= ControlPointerLeave;
                    if (control.Parent != null)
                    {
                        control.Parent.PointerLeave -= parentControlLeave;
                        control.Parent.PointerEnter -= parentControlEnter;
                    }
                }

                if (e.NewValue != null)
                {
                    control.PointerEnter += ControlPointerEnter;
                    control.PointerLeave += ControlPointerLeave;
                    if (control.Parent != null)
                    {
                        control.Parent.PointerLeave += parentControlLeave;
                        control.Parent.PointerEnter += parentControlEnter;
                    }
                }
            }

            /// <summary>
            /// Closes the specified control.
            /// </summary>
            /// <param name="control">The control.</param>
            private void Close(Control control)
            {
                StopTimer();

                SetIsOpen(control, false);
            }

            /// <summary>
            /// Called when the pointer enters a control with an attached tooltip.
            /// </summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The event args.</param>
            private void ControlPointerEnter(object sender, PointerEventArgs e)
            {
                HandleControlPointerEnter(sender, false);
            }

            /// <summary>
            /// Called when the pointer leaves a control with an attached tooltip.
            /// </summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The event args.</param>
            private void ControlPointerLeave(object sender, PointerEventArgs e)
            {
                HandleControlPointerLeave(sender, false);
            }

            /// <summary>
            /// Handles the control pointer enter.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="skipParentCheck">if set to <c>true</c> [skip parent check].</param>
            private void HandleControlPointerEnter(object sender, bool skipParentCheck)
            {
                var control = (Control)sender;
                if (!skipParentCheck)
                {
                    var parentPointerOver = (control.Parent?.IsPointerOver).GetValueOrDefault();
                    var pointerOver = control.IsPointerOver;
                    var reactToParent = GetReactToParent(control);
                    if (reactToParent && (parentPointerOver || pointerOver))
                    {
                        return;
                    }
                }

                if (control.IsVisible && !GetIsOpen(control))
                {
                    StopTimer();

                    var showDelay = GetShowDelay(control);
                    if (showDelay == 0)
                    {
                        Open(control);
                    }
                    else
                    {
                        StartShowTimer(showDelay, control);
                    }
                }
            }

            /// <summary>
            /// Handles the control pointer leave.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="skipParentCheck">if set to <c>true</c> [skip parent check].</param>
            private void HandleControlPointerLeave(object sender, bool skipParentCheck)
            {
                var control = (Control)sender;
                var parentPointerOver = (control.Parent?.IsPointerOver).GetValueOrDefault();
                var pointerOver = control.IsPointerOver;
                if (!skipParentCheck)
                {
                    var reactToParent = GetReactToParent(control);
                    if (reactToParent && (parentPointerOver || pointerOver))
                    {
                        return;
                    }
                }
                if (control.IsVisible && GetIsOpen(control))
                {
                    Close(control);
                }
            }

            /// <summary>
            /// Opens the specified control.
            /// </summary>
            /// <param name="control">The control.</param>
            private void Open(Control control)
            {
                StopTimer();

                if ((control as IVisual).IsAttachedToVisualTree)
                {
                    SetIsOpen(control, true);
                }
            }

            /// <summary>
            /// Starts the show timer.
            /// </summary>
            /// <param name="showDelay">The show delay.</param>
            /// <param name="control">The control.</param>
            private void StartShowTimer(int showDelay, Control control)
            {
                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(showDelay) };
                _timer.Tick += (o, e) => Open(control);
                _timer.Start();
            }

            /// <summary>
            /// Stops the timer.
            /// </summary>
            private void StopTimer()
            {
                _timer?.Stop();
                _timer = null;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
