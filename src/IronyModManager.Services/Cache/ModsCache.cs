// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2020
// ***********************************************************************
// <copyright file="ModsCache.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Cache
{
    /// <summary>
    /// Class ModsCache.
    /// </summary>
    internal static class ModsCache
    {
        #region Fields

        /// <summary>
        /// The service lock
        /// </summary>
        private readonly static object serviceLock = new { };

        /// <summary>
        /// All mods
        /// </summary>
        private static Dictionary<string, List<IMod>> allMods = new Dictionary<string, List<IMod>>();

        /// <summary>
        /// The mapper
        /// </summary>
        private static IMapper mapper;

        /// <summary>
        /// The regular mods
        /// </summary>
        private static Dictionary<string, List<IMod>> regularMods = new Dictionary<string, List<IMod>>();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets all mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>List&lt;IMod&gt;.</returns>
        public static List<IMod> GetAllMods(string game)
        {
            InitMapper();
            if (allMods.ContainsKey(game))
            {
                var mods = new List<IMod>();
                mapper.Map(allMods[game], mods);
                return mods;
            }
            return null;
        }

        /// <summary>
        /// Gets the regular mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>List&lt;IMod&gt;.</returns>
        public static List<IMod> GetRegularMods(string game)
        {
            InitMapper();
            if (regularMods.ContainsKey(game))
            {
                var mods = new List<IMod>();
                mapper.Map(regularMods[game], mods);
                return mods;
            }
            return null;
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="game">The game.</param>
        public static void InvalidateCache(IGame game)
        {
            if (game != null)
            {
                lock (serviceLock)
                {
                    if (allMods.ContainsKey(game.Type))
                    {
                        allMods.Remove(game.Type);
                    }
                    if (regularMods.ContainsKey(game.Type))
                    {
                        regularMods.Remove(game.Type);
                    }
                }
            }
        }

        /// <summary>
        /// Sets all mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        public static void SetAllMods(string game, List<IMod> mods)
        {
            lock (serviceLock)
            {
                if (allMods.ContainsKey(game))
                {
                    allMods[game] = mods;
                }
                else
                {
                    allMods.Add(game, mods);
                }
            }
        }

        /// <summary>
        /// Sets the regular mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        public static void SetRegularMods(string game, List<IMod> mods)
        {
            lock (serviceLock)
            {
                if (regularMods.ContainsKey(game))
                {
                    regularMods[game] = mods;
                }
                else
                {
                    regularMods.Add(game, mods);
                }
            }
        }

        /// <summary>
        /// Initializes the mapper.
        /// </summary>
        private static void InitMapper()
        {
            if (mapper == null)
            {
                mapper = DIResolver.Get<IMapper>();
            }
        }

        #endregion Methods
    }
}
