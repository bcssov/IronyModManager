// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2020
// ***********************************************************************
// <copyright file="MappingProfile.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Shared;

namespace IronyModManager.Models
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
            CreateMap<IPreferences, Preferences>().ReverseMap();
            CreateMap<ITheme, Theme>().ReverseMap();
            CreateMap<ILanguage, Language>().ReverseMap();
            CreateMap<IWindowState, WindowState>().ReverseMap();
            CreateMap<IGame, Game>().ReverseMap();
            CreateMap<IMod, Mod>().ReverseMap();
            CreateMap<IModObject, IMod>().ReverseMap();
            CreateMap<IModObject, Mod>().ReverseMap();
            CreateMap<IAppState, AppState>().ReverseMap();
            CreateMap<IModCollection, ModCollection>().ReverseMap();
            CreateMap<IConflictResult, ConflictResult>().ReverseMap();
        }

        #endregion Constructors
    }
}
