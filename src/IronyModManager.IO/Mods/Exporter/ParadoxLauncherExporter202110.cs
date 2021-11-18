// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
// ***********************************************************************
// <copyright file="ParadoxLauncherExporter202110.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.Json.v3;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Mods.Exporter
{
    /// <summary>
    /// Class ParadoxLauncherExporter202110.
    /// Implements the <see cref="IronyModManager.IO.Mods.Exporter.BaseExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Exporter.BaseExporter" />
    internal class ParadoxLauncherExporter202110 : BaseExporter
    {
        #region Methods

        /// <summary>
        /// Export as an asynchronous operation.
        /// </summary>
        /// <param name="params">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async Task<bool> ExportAsync(ModCollectionExporterParams @params)
        {
            // Paradox launcher only exports pdx or steam mods, local mods are not exported
            var validMods = @params.ExportMods.Where(p => p.Source != ModSource.Local).ToList();
            var modInfo = new ModInfo()
            {
                Game = @params.Game.ParadoxGameId,
                Name = @params.Mod.Name,
                Mods = validMods.Select(p => new Models.Paradox.Json.v3.Mods()
                {
                    DisplayName = p.Name,
                    Enabled = true,
                    Position = validMods.IndexOf(p),
                    SteamId = p.Source == ModSource.Steam ? p.RemoteId.ToString() : null,
                    PdxId = p.Source == ModSource.Paradox ? p.RemoteId.ToString() : null
                }).ToList()
            };
            var json = JsonDISerializer.Serialize(modInfo);
            await File.WriteAllTextAsync(@params.File, json);
            return true;
        }

        #endregion Methods
    }
}
