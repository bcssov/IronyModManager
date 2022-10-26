// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 10-26-2022
// ***********************************************************************
// <copyright file="Extensions.Automapper.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Ignores all unmapped members.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TDestination">The type of the t destination.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression&lt;TSource, TDestination&gt;.</returns>
        public static void IgnoreAllUnmappedMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            // Code smell? Yes. Why? A BS excuse from automapper to remove ForAllOtherMembers.
            var memberConfigurations = expression.GetType().GetProperty("MemberConfigurations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(expression) as List<IPropertyMapConfiguration>;
            var typeMapActions = expression.GetType().GetProperty("TypeMapActions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(expression) as List<Action<TypeMap>>;
            var types = ((TypeMapConfiguration)expression).Types;
            bool hasDestinationMemberConfiguration(string name)
            {
                return memberConfigurations.Any(m => m.DestinationMember.Name == name);
            }
            typeMapActions.Add(typeMap =>
            {
                foreach (var accessor in typeMap.DestinationSetters.Where(m => !hasDestinationMemberConfiguration(m.Name)))
                {
                    var exp = new MemberConfigurationExpression(accessor, types.SourceType);
                    memberConfigurations.Add(exp);
                    exp.Ignore();
                }
            });
        }

        #endregion Methods
    }
}
