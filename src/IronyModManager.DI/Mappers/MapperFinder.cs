// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="MapperFinder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IronyModManager.Shared;

namespace IronyModManager.DI.Mappers
{
    /// <summary>
    /// Class MapperRegistry.
    /// </summary>
    internal static class MapperFinder
    {
        #region Fields

        /// <summary>
        /// The resolver
        /// </summary>
        private static readonly Func<TypeMap, object, object> resolver = (t, x) =>
           {
               var type = x.GetType();
               var derivedType = GetDerivedTypeFor(t, type);
               if (derivedType.IsInterface)
               {
                   return DIResolver.Get(derivedType);
               }
               return derivedType;
           };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Finds the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>MapperConfiguration.</returns>
        public static MapperConfiguration Find(List<Assembly> assemblies)
        {
            var config = new MapperConfiguration(cfg =>
            {
                CompileOptions<BaseMappingProfile>(assemblies, cfg);
                CompileOptions<BaseMappingProfileOverride>(assemblies, cfg);
            });
            return config;
        }

        /// <summary>
        /// Compiles the options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="cfg">The CFG.</param>
        private static void CompileOptions<T>(List<Assembly> assemblies, IMapperConfigurationExpression cfg) where T : Profile
        {
            var profiles = assemblies.Select(p => p.GetTypes().Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract));

            cfg.ConstructServicesUsing((s) => DIResolver.Get(s));

            foreach (var assemblyProfiles in profiles)
            {
                foreach (var assemblyProfile in assemblyProfiles)
                {
                    cfg.AddProfile(Activator.CreateInstance(assemblyProfile) as T);
                }
            }
            var internalCfg = AutoMapper.Internal.InternalApi.Internal(cfg);
            internalCfg.ForAllMaps((t, m) =>
            {
                m.ConstructUsing((x) => resolver(t, x));
            });
        }

        /// <summary>
        /// Gets the derived type for.
        /// </summary>
        /// <param name="typeMap">The type map.</param>
        /// <param name="derivedSourceType">Type of the derived source.</param>
        /// <returns>Type.</returns>
        private static Type GetDerivedTypeFor(TypeMap typeMap, Type derivedSourceType)
        {
            // Reversed from automapper
            if (typeMap.DestinationTypeOverride != null)
            {
                return typeMap.DestinationTypeOverride;
            }
            var match = typeMap.IncludedDerivedTypes.FirstOrDefault(tp => tp.SourceType == derivedSourceType);

            return match.DestinationType ?? typeMap.DestinationType;
        }

        #endregion Methods
    }
}
