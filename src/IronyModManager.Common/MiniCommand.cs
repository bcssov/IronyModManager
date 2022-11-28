// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 11-28-2022
//
// Last Modified By : Mario
// Last Modified On : 11-28-2022
// ***********************************************************************
// <copyright file="MiniCommand.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IronyModManager.Common
{
    /// <summary>
    /// Class MiniCommand. This class cannot be inherited.
    /// Implements the <see cref="IronyModManager.Common.MiniCommand" />
    /// Implements the <see cref="ICommand" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IronyModManager.Common.MiniCommand" />
    /// <seealso cref="ICommand" />
    public sealed class MiniCommand<T> : MiniCommand, ICommand
    {
        #region Fields

        /// <summary>
        /// The cb
        /// </summary>
        private readonly Action<T> cb;

        /// <summary>
        /// The busy
        /// </summary>
        private bool _busy;

        /// <summary>
        /// The acb
        /// </summary>
        private Func<T, Task> acb;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniCommand{T}" /> class.
        /// </summary>
        /// <param name="cb">The cb.</param>
        public MiniCommand(Action<T> cb)
        {
            this.cb = cb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniCommand{T}" /> class.
        /// </summary>
        /// <param name="cb">The cb.</param>
        public MiniCommand(Func<T, Task> cb)
        {
            acb = cb;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public override event EventHandler CanExecuteChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MiniCommand{T}" /> is busy.
        /// </summary>
        /// <value><c>true</c> if busy; otherwise, <c>false</c>.</value>
        private bool Busy
        {
            get => _busy;
            set
            {
                _busy = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
        public override bool CanExecute(object parameter) => !_busy;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public override async void Execute(object parameter)
        {
            if (Busy)
                return;
            try
            {
                Busy = true;
                if (cb != null)
                    cb((T)parameter);
                else
                    await acb((T)parameter);
            }
            finally
            {
                Busy = false;
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// Class MiniCommand.
    /// Implements the <see cref="IronyModManager.Common.MiniCommand" />
    /// Implements the <see cref="ICommand" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.MiniCommand" />
    /// <seealso cref="ICommand" />
    public abstract class MiniCommand : ICommand
    {
        #region Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public abstract event EventHandler CanExecuteChanged;

        #endregion Events

        #region Methods

        /// <summary>
        /// Creates the specified cb.
        /// </summary>
        /// <param name="cb">The cb.</param>
        /// <returns>MiniCommand.</returns>
        public static MiniCommand Create(Action cb) => new MiniCommand<object>(_ => cb());

        /// <summary>
        /// Creates the specified cb.
        /// </summary>
        /// <typeparam name="TArg">The type of the t argument.</typeparam>
        /// <param name="cb">The cb.</param>
        /// <returns>MiniCommand.</returns>
        public static MiniCommand Create<TArg>(Action<TArg> cb) => new MiniCommand<TArg>(cb);

        /// <summary>
        /// Creates from task.
        /// </summary>
        /// <param name="cb">The cb.</param>
        /// <returns>MiniCommand.</returns>
        public static MiniCommand CreateFromTask(Func<Task> cb) => new MiniCommand<object>(_ => cb());

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public abstract void Execute(object parameter);

        #endregion Methods
    }
}
