// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-11-2020
// ***********************************************************************
// <copyright file="BaseExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using IronyModManager.IO.Mods.Models.Paradox;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Mods.Exporter
{
    /// <summary>
    /// Class BaseExporter.
    /// </summary>
    internal abstract class BaseExporter
    {
        #region Methods

        /// <summary>
        /// Maps the mod data.
        /// </summary>
        /// <param name="pdxMod">The PDX mod.</param>
        /// <param name="mod">The mod.</param>
        protected virtual void MapModData(IPdxMod pdxMod, IMod mod)
        {
            pdxMod.DisplayName = mod.Name;
            pdxMod.Tags = mod.Tags?.ToList();
            pdxMod.RequiredVersion = mod.Version;
            pdxMod.GameRegistryId = mod.DescriptorFile;
            pdxMod.Status = Constants.Ready_to_play;
            pdxMod.Source = MapPdxType(mod.Source);
            MapPdxPath(pdxMod, mod);
            MapPdxId(pdxMod, mod);
        }

        /// <summary>
        /// Maps the PDX identifier.
        /// </summary>
        /// <param name="pdxMod">The PDX mod.</param>
        /// <param name="mod">The mod.</param>
        protected virtual void MapPdxId(IPdxMod pdxMod, IMod mod)
        {
            if (mod.RemoteId.HasValue)
            {
                switch (mod.Source)
                {
                    case ModSource.Paradox:
                        pdxMod.PdxId = mod.RemoteId.ToString();
                        break;

                    default:
                        // Assume steam
                        pdxMod.SteamId = mod.RemoteId.ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// Maps the PDX path.
        /// </summary>
        /// <param name="pdxMod">The PDX mod.</param>
        /// <param name="mod">The mod.</param>
        protected virtual void MapPdxPath(IPdxMod pdxMod, IMod mod)
        {
            if (mod.FileName.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase))
            {
                pdxMod.ArchivePath = mod.FileName;
                if (mod.Source != ModSource.Local)
                {
                    pdxMod.DirPath = Path.GetDirectoryName(mod.FileName);
                }
            }
            else
            {
                pdxMod.DirPath = mod.FileName;
            }
        }

        /// <summary>
        /// Maps the type of the PDX.
        /// </summary>
        /// <param name="modSource">The mod source.</param>
        /// <returns>System.String.</returns>
        protected virtual string MapPdxType(ModSource modSource)
        {
            var pdxSource = modSource switch
            {
                ModSource.Paradox => "pdx",
                ModSource.Steam => "steam",
                _ => "local",
            };
            return pdxSource;
        }

        #endregion Methods
    }
}
