// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="BoundClassBorder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class BinaryDiffBorder.
    /// Implements the <see cref="Avalonia.Controls.Border" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Border" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class BoundClassBorder : Border, IStyleable
    {
        #region Fields

        /// <summary>
        /// The bound classes property
        /// </summary>
        public static readonly StyledProperty<string> BoundClassesProperty = AvaloniaProperty.Register<BoundClassBorder, string>(nameof(BoundClasses));

        /// <summary>
        /// The old classes
        /// </summary>
        private string[] oldClasses = null;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the bound classes.
        /// </summary>
        /// <value>The bound classes.</value>
        public string BoundClasses
        {
            get
            {
                return GetValue(BoundClassesProperty);
            }
            set
            {
                SetValue(BoundClassesProperty, value);
                AddClasses(value);
            }
        }

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Border);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the classes.
        /// </summary>
        /// <param name="classes">The classes.</param>
        protected virtual void AddClasses(string classes)
        {
            if (oldClasses != null)
            {
                Classes.RemoveAll(oldClasses);
            }
            if (!string.IsNullOrWhiteSpace(classes))
            {
                // I swear to God every time I hit a wall with Avalonia I lose a piece of my mind. So good in certain areas but so annoyingly lacking in others...
                var classArray = classes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Classes.AddRange(classArray);
                oldClasses = classArray;
            }
            else
            {
                oldClasses = null;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BoundClassesProperty)
            {
                AddClasses(e.NewValue != null ? e.NewValue.ToString() : string.Empty);
            }
        }

        #endregion Methods
    }
}
