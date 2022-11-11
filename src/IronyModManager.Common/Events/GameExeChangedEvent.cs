// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 10-29-2022
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="GameExeChangedEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class GameExeChangedEvent.
    /// Implements the <see cref="BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="BaseNonAwaitableEvent" />
    public class GameExeChangedEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameExeChangedEvent"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public GameExeChangedEvent(string path)
        {
            Path = path;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; private set; }

        #endregion Properties

    }
}
