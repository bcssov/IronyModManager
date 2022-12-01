// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 12-01-2022
// ***********************************************************************
// <copyright file="BaseExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.IO.Mods.Models.Paradox;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods.Exporter
{
    /// <summary>
    /// Class BaseExporter.
    /// </summary>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal abstract class BaseExporter
    {
        #region Methods

        /// <summary>
        /// Maps the name of the file.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        protected virtual string MapFileName(IMod mod)
        {
            return mod.FileName;
        }

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
            if (mod.FileName.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase) ||
                mod.FileName.EndsWith(Shared.Constants.BinExtension, StringComparison.OrdinalIgnoreCase))
            {
                pdxMod.ArchivePath = MapFileName(mod);
                if (mod.Source != ModSource.Local)
                {
                    pdxMod.DirPath = Path.GetDirectoryName(MapFileName(mod));
                }
            }
            else
            {
                pdxMod.DirPath = MapFileName(mod);
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
