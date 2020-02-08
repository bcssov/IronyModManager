// ***********************************************************************
// Assembly         : IronyModManager.Tests.Common
// Author           : Mario
// Created          : 01-29-2020
//
// Last Modified By : Mario
// Last Modified On : 02-09-2020
// ***********************************************************************
// <copyright file="ExcludeFromCoverageAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class ExcludeFromCoverageAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [ExcludeFromCodeCoverage]
    public class ExcludeFromCoverageAttribute : Attribute
    {
        #region Fields

        /// <summary>
        /// The reason
        /// </summary>
        private readonly string reason;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeFromCoverageWithReason" /> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public ExcludeFromCoverageAttribute(string reason)
        {
            this.reason = reason;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>The reason.</value>
        public virtual string Reason
        {
            get
            {
                return reason;
            }
        }

        #endregion Properties
    }
}
