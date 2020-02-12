// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="MappingProfile.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class MappingProfile.
    /// Implements the <see cref="IronyModManager.Shared.BaseMappingProfile" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.BaseMappingProfile" />
    [ExcludeFromCoverage("Mapping profile shouldn't be tested.")]
    public class MappingProfile : BaseMappingProfile
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile" /> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<IThemeType, ThemeType>().ReverseMap();
            CreateMap<IDatabase, Database>().ReverseMap();
            CreateMap<IGameType, GameType>().ReverseMap();
            CreateMap<IPreferences, IPreferences>().ReverseMap();
            CreateMap<IWindowState, IWindowState>().ReverseMap();
        }

        #endregion Constructors
    }
}
