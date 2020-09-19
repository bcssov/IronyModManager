// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 09-19-2020
// ***********************************************************************
// <copyright file="MappingProfile.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
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
            CreateMap<ITheme, IPreferences>()
                .ForMember(p => p.Theme, o => o.MapFrom(m => m.Type)).ReverseMap().ForAllMembers(p => p.Ignore());
            CreateMap<ILanguage, IPreferences>()
                .ForMember(p => p.Locale, o => o.MapFrom(m => m.Abrv)).ReverseMap().ForAllMembers(p => p.Ignore());
            CreateMap<IGame, IPreferences>()
                .ForMember(m => m.Game, o => o.MapFrom(s => s.Type)).ReverseMap().ForAllMembers(m => m.Ignore());
            CreateMap<IUpdateSettings, IPreferences>()
                .ForMember(m => m.AutoUpdates, o => o.MapFrom(s => s.AutoUpdates))
                .ForMember(m => m.CheckForPrerelease, o => o.MapFrom(s => s.CheckForPrerelease))
                .ReverseMap().ForAllMembers(m => m.Ignore());
            CreateMap<IPreferences, IUpdateSettings>()
                .ForMember(m => m.AutoUpdates, o => o.MapFrom(s => s.AutoUpdates))
                .ForMember(m => m.CheckForPrerelease, o => o.MapFrom(s => s.CheckForPrerelease))
                .ReverseMap().ForAllMembers(m => m.Ignore());
            CreateMap<IDefinition, IDefinition>().ReverseMap();
            CreateMap<IMod, IMod>().ReverseMap();
        }

        #endregion Constructors
    }
}
