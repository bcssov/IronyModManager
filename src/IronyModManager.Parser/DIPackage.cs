// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.DI.Extensions;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Common.Mod.Search.Converter;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Parser.Default;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.DLC;
using IronyModManager.Parser.Games.HOI4;
using IronyModManager.Parser.Games.Stellaris;
using IronyModManager.Parser.Generic;
using IronyModManager.Parser.Mod;
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Parser.Mod.Search.Converter;
using IronyModManager.Parser.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public class DIPackage : IPackage
    {
        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.Register<IDefinition, Definition>();
            container.Register<IIndexedDefinitions, IndexedDefinitions>();
            container.RemoveTransientWarning<IIndexedDefinitions>();
            container.Collection.Register(typeof(IDefaultParser), new List<Type>()
            {
                typeof(DefaultParser)
            });
            container.Collection.Register(typeof(IGenericParser), new List<Type>()
            {
                 typeof(BinaryParser), typeof(DefinesParser), typeof(GraphicsParser),
                 typeof(KeyParser), typeof(LocalizationParser), typeof(Generic.WholeTextParser)
            });
            container.Collection.Register(typeof(IGameParser), new List<Type>
            {
                typeof(FlagsParser), typeof(SolarSystemInitializersParser), typeof(Games.Stellaris.WholeTextParser),
                typeof(OverwrittenParser), typeof(ScriptedVariablesParser), typeof(OverwrittenObjectSingleFileParser),
                typeof(KeyValuePairParser)
            });
            container.Register<IParserManager, ParserManager>();
            container.Register<IModObject, ModObject>();
            container.Register<IModParser, ModParser>();
            container.Register<ICodeParser, CodeParser>();
            container.Register<IValidateParser, ValidateParser>();
            container.Register<IHierarchicalDefinitions, HierarchicalDefinitions>();
            container.Register<IParserMap, ParserMap>();
            container.Register<IScriptElement, ScriptElement>();
            container.Register<IScriptError, ScriptError>();
            container.Register<IParseResponse, ParseResponse>();
            container.Register<IDLCParser, DLCParser>();
            container.Register<IDLCObject, DLCObject>();
            container.Register<IBracketValidateResult, BracketValidateResult>();
            container.Register<ISearchParserResult, SearchParserResult>();
            container.Register<ILocalizationRegistry, LocalizationRegistry>(Lifestyle.Singleton);
            container.Collection.Register(typeof(ITypeConverter<>), new List<Type>
            {
                typeof(BoolConverter), typeof(VersionConverter), typeof(SourceTypeConverter)
            });
            container.Register<IParser, Mod.Search.Parser>();
        }

        #endregion Methods
    }
}
