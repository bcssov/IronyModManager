// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 06-23-2021
//
// Last Modified By : Mario
// Last Modified On : 06-23-2021
// ***********************************************************************
// <copyright file="InputManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Input.Raw;

namespace IronyModManager.Platform.InputManager
{
    /// <summary>
    /// Class InputManager.
    /// Implements the <see cref="Avalonia.Input.IInputManager" />
    /// </summary>
    /// <seealso cref="Avalonia.Input.IInputManager" />
    internal class InputManager : IInputManager
    {
        #region Fields

        /// <summary>
        /// The underlying input manager
        /// </summary>
        private readonly IInputManager underlyingInputManager;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// </summary>
        /// <param name="underlyingInputManager">The underlying input manager.</param>
        public InputManager(IInputManager underlyingInputManager)
        {
            this.underlyingInputManager = underlyingInputManager;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets an observable that notifies on each input event received after
        /// <see cref="P:Avalonia.Input.IInputManager.Process" />.
        /// </summary>
        /// <value>The post process.</value>
        public IObservable<RawInputEventArgs> PostProcess => underlyingInputManager.PostProcess;

        /// <summary>
        /// Gets an observable that notifies on each input event received before
        /// <see cref="P:Avalonia.Input.IInputManager.Process" />.
        /// </summary>
        /// <value>The pre process.</value>
        public IObservable<RawInputEventArgs> PreProcess => underlyingInputManager.PreProcess;

        /// <summary>
        /// Gets an observable that notifies on each input event received.
        /// </summary>
        /// <value>The process.</value>
        public IObservable<RawInputEventArgs> Process => underlyingInputManager.Process;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Processes a raw input event.
        /// </summary>
        /// <param name="e">The raw input event.</param>
        public void ProcessInput(RawInputEventArgs e)
        {
            if (e.Device != null && e.Device is IKeyboardDevice)
            {
                try
                {
                    // This got broken somehow
                    underlyingInputManager.ProcessInput(e);
                }
                catch (InvalidOperationException)
                {
                }
            }
            else
            {
                underlyingInputManager.ProcessInput(e);
            }
        }

        #endregion Methods
    }
}
