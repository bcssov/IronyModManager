// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 05-12-2020
//
// Last Modified By : Mario
// Last Modified On : 05-12-2020
// ***********************************************************************
// <copyright file="RetryStrategy.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class RetryStrategy.
    /// </summary>
    public class RetryStrategy
    {
        #region Fields

        /// <summary>
        /// The retry attempts
        /// </summary>
        public const int RetryAttempts = 10;

        /// <summary>
        /// The retry delay
        /// </summary>
        public const int RetryDelay = 250;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryStrategy" /> class.
        /// </summary>
        /// <param name="retryAttempts">The retry attempts.</param>
        /// <param name="retryDelay">The retry delay.</param>
        public RetryStrategy(int retryAttempts = RetryAttempts, int retryDelay = RetryDelay)
        {
            Attempts = retryAttempts;
            Delay = retryDelay;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the attempts.
        /// </summary>
        /// <value>The attempts.</value>
        public int Attempts { get; private set; }

        /// <summary>
        /// Gets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// retry action as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>T.</returns>
        /// <exception cref="AggregateException">Retry strategy failed.</exception>
        public async Task<T> RetryActionAsync<T>(Func<Task<T>> action)
        {
            var attempts = 1;
            T result = default;
            bool strategySuccessful = false;
            var exceptions = new List<Exception>();
            while (attempts <= Attempts)
            {
                try
                {
                    result = await action();
                    strategySuccessful = true;
                    break;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                attempts++;
                await Task.Delay(Delay);
            }
            if (strategySuccessful)
            {
                return result;
            }
            throw new AggregateException("Retry strategy failed.", exceptions);
        }

        #endregion Methods
    }
}
