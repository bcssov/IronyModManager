// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2021
//
// Last Modified By : Mario
// Last Modified On : 09-16-2021
// ***********************************************************************
// <copyright file="ScrollThumb.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using IronyModManager.DI;
using IronyModManager.Implementation.AppState;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class ScrollThumb.
    /// Implements the <see cref="Avalonia.Controls.Primitives.Thumb" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Primitives.Thumb" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class ScrollThumb : Thumb, IStyleable
    {
        #region Fields

        /// <summary>
        /// The last point
        /// </summary>
        private Point? lastPoint;

        /// <summary>
        /// The scroll state
        /// </summary>
        private IScrollState scrollState;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollThumb"/> class.
        /// </summary>
        public ScrollThumb() : base()
        {
            GetScrollState().State.Subscribe(s =>
            {
                if (!s)
                {
                    lastPoint = null;
                    PseudoClasses.Remove(":pressed");
                }
            });
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Thumb);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:PointerCaptureLost" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerCaptureLostEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            if (lastPoint.HasValue)
            {
                var ev = new VectorEventArgs
                {
                    RoutedEvent = DragCompletedEvent,
                    Vector = lastPoint.Value,
                };

                lastPoint = null;

                RaiseEvent(ev);
            }

            PseudoClasses.Remove(":pressed");

            base.OnPointerCaptureLost(e);
        }

        /// <summary>
        /// Handles the <see cref="E:PointerMoved" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (lastPoint.HasValue)
            {
                var ev = new VectorEventArgs
                {
                    RoutedEvent = DragDeltaEvent,
                    Vector = e.GetPosition(this) - lastPoint.Value,
                };

                RaiseEvent(ev);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            e.Handled = true;
            lastPoint = e.GetPosition(this);

            var ev = new VectorEventArgs
            {
                RoutedEvent = DragStartedEvent,
                Vector = (Vector)lastPoint,
            };

            PseudoClasses.Add(":pressed");

            RaiseEvent(ev);
        }

        /// <summary>
        /// Handles the <see cref="E:PointerReleased" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerReleasedEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (lastPoint.HasValue)
            {
                e.Handled = true;
                lastPoint = null;

                var ev = new VectorEventArgs
                {
                    RoutedEvent = DragCompletedEvent,
                    Vector = e.GetPosition(this),
                };

                RaiseEvent(ev);
            }

            PseudoClasses.Remove(":pressed");
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
