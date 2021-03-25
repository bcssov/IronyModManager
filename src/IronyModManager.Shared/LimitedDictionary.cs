// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-25-2021
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="LimitedDictionary.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class LimitedDictionary.
    /// Implements the <see cref="System.Collections.Generic.Dictionary{TKey, TValue}" />
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="System.Collections.Generic.Dictionary{TKey, TValue}" />
    public class LimitedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the maximum items.
        /// </summary>
        /// <value>The maximum items.</value>
        public int? MaxItems { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ensures the maximum items.
        /// </summary>
        public void EnsureMaxItems()
        {
            if (MaxItems.GetValueOrDefault() > 0 && MaxItems.GetValueOrDefault() < Count)
            {
                while (MaxItems.GetValueOrDefault() < Count)
                {
                    Remove(this.FirstOrDefault().Key);
                }
            }
        }

        #endregion Methods
    }
}
