// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-18-2026
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="GameInfoRequestHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.Services.Common;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Services.MessageBus
{
    /// <summary>
    /// Class GameInfoRequestHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.IO.Common.MessageBus.GameInfoRequestEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.IO.Common.MessageBus.GameInfoRequestEvent}" />
    public class GameInfoRequestHandler : BaseMessageBusConsumer<GameInfoRequestEvent>
    {
        #region Fields

        /// <summary>
        /// The game service
        /// </summary>
        private static IGameService gameService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Called when [handle].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public override Task OnHandle(GameInfoRequestEvent message)
        {
            Populate(message);
            return base.OnHandle(message);
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        /// <exception cref="System.NotSupportedException">Cannot subscribe.</exception>
        public override IDisposable Subscribe(Action<GameInfoRequestEvent> action)
        {
            throw new NotSupportedException("Cannot subscribe.");
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        /// <exception cref="System.NotSupportedException">Cannot subscribe.</exception>
        public override IDisposable Subscribe(Func<GameInfoRequestEvent, Task> action)
        {
            throw new NotSupportedException("Cannot subscribe.");
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        /// <exception cref="System.NotSupportedException">Cannot subscribe.</exception>
        public override IDisposable Subscribe<T>(Func<GameInfoRequestEvent, Task<T>> action)
        {
            throw new NotSupportedException("Cannot subscribe.");
        }

        /// <summary>
        /// Populates the specified parameters.
        /// </summary>
        /// <param name="params">The parameters.</param>
        private void Populate(GameInfoRequestEvent @params)
        {
            gameService ??= DIResolver.Get<IGameService>();
            var games = gameService.Get();
            var game = games?.FirstOrDefault(p => p.Type.Equals(@params.GameId, StringComparison.OrdinalIgnoreCase));
            if (game != null)
            {
                @params.BaseSteamDir = game.BaseSteamGameDirectory;
                @params.IsProton = !string.IsNullOrWhiteSpace(game.LinuxProtonVersion);
                @params.SteamAppId = game.SteamAppId;
            }
        }

        #endregion Methods
    }
}
