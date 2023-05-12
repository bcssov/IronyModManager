// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-12-2023
//
// Last Modified By : Mario
// Last Modified On : 05-12-2023
// ***********************************************************************
// <copyright file="VerticalMenuItem.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class VerticalMenuItem.
    /// Implements the <see cref="MenuItem" />
    /// Implements the <see cref="IStyleable" />
    /// </summary>
    /// <seealso cref="MenuItem" />
    /// <seealso cref="IStyleable" />
    public class VerticalMenuItem : MenuItem, IStyleable
    {
        #region Fields

        /// <summary>
        /// The automatic open property
        /// </summary>
        public static readonly DirectProperty<VerticalMenuItem, bool> AutoOpenProperty = AvaloniaProperty.RegisterDirect<VerticalMenuItem, bool>(nameof(AutoOpen), o => o.AutoOpen, (o, v) => o.AutoOpen = v);

        /// <summary>
        /// The automatic open
        /// </summary>
        private bool autoOpen = true;

        /// <summary>
        /// The popup
        /// </summary>
        private Popup popup;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [automatic open].
        /// </summary>
        /// <value><c>true</c> if [automatic open]; otherwise, <c>false</c>.</value>
        public bool AutoOpen
        {
            get { return autoOpen; }
            set { SetAndRaise(AutoOpenProperty, ref autoOpen, value); }
        }

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(MenuItem);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs"/> instance containing the event data.</param>
        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            popup = e.NameScope.Find<Popup>("PART_Popup");

            if (popup != null)
            {
                popup.PlacementMode = PlacementMode.Right;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerEnter" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event data.</param>
        /// <inheritdoc />
        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            if (AutoOpen && IsPointerOver)
            {
                var hasChildren = this.GetLogicalChildren().OfType<MenuItem>().Any();
                if (hasChildren)
                {
                    Open();
                }
            }
        }

        #endregion Methods
    }
}
