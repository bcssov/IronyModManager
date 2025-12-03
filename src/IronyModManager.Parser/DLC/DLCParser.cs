// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="DLCParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared.Models;
using Newtonsoft.Json;

namespace IronyModManager.Parser.DLC
{
    /// <summary>
    /// Class DLCParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.DLC.IDLCParser" />
    /// Implements the <see cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// <seealso cref="IronyModManager.Parser.Common.DLC.IDLCParser" />
    /// <remarks>Initializes a new instance of the <see cref="DLCParser" /> class.</remarks>
    public class DLCParser(ICodeParser codeParser) : BaseGenericObjectParser(codeParser), IDLCParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified lines.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="descriptorModType">Type of the descriptor mod.</param>
        /// <returns>IDLCObject.</returns>
        public IDLCObject Parse(string path, IEnumerable<string> lines, DescriptorModType descriptorModType)
        {
            var obj = DIResolver.Get<IDLCObject>();
            obj.IsVisible = true;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (descriptorModType)
            {
                case DescriptorModType.DescriptorMod:
                {
                    var data = ParseCode(lines);
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = string.Empty;
                    }

                    if (data != null)
                    {
                        obj.Name = GetValue<string>(data.Values, "name") ?? string.Empty;
                        obj.Path = path.Replace("\\", "/");
                    }

                    break;
                }
                case DescriptorModType.JsonMetadata:
                {
                    var json = string.Join(Environment.NewLine, lines);
                    var result = JsonConvert.DeserializeObject<DLCObject>(json, new JsonSerializerSettings { Error = (_, error) => error.ErrorContext.Handled = true, NullValueHandling = NullValueHandling.Ignore });
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = string.Empty;
                    }

                    obj.Path = path.Replace("\\", "/");
                    obj.AppId = result.Id != null ? result.Id.Paradox : string.Empty;
                    obj.Name = result.DisplayName != null ? result.DisplayName.En : string.Empty;
                    break;
                }
                default:
                {
                    var json = string.Join(Environment.NewLine, lines);
                    var result = JsonConvert.DeserializeObject<DLCObjectV2>(json, new JsonSerializerSettings { Error = (_, error) => error.ErrorContext.Handled = true, NullValueHandling = NullValueHandling.Ignore });
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = string.Empty;
                    }

                    obj.Path = path.Replace("\\", "/");
                    obj.AppId = !string.IsNullOrWhiteSpace(result.Name) ? result.Name : string.Empty;
                    obj.Name = !string.IsNullOrWhiteSpace(result.Name) ? result.Name : string.Empty;
                    obj.IsVisible = !result.Hidden;
                    break;
                }
            }

            return obj;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DLCObjectV2.
        /// </summary>
        public class DLCObjectV2
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="DLCObjectV2" /> is hidden.
            /// </summary>
            /// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
            [JsonProperty("hidden")]
            public bool Hidden { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [JsonProperty("name")]
            public string Name { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class DisplayName.
        /// </summary>
        private class DisplayName
        {
            #region Properties

            /// <summary>
            /// Gets or sets the en.
            /// </summary>
            /// <value>The en.</value>
            [JsonProperty("en")]
            public string En { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class DLCObject.
        /// </summary>
        private class DLCObject
        {
            #region Properties

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            /// <value>The display name.</value>
            [JsonProperty("displayName")]
            public DisplayName DisplayName { get; set; }

            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            [JsonProperty("id")]
            public Id Id { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class ID.
        /// </summary>
        private class Id
        {
            #region Properties

            /// <summary>
            /// Gets or sets the paradox.
            /// </summary>
            /// <value>The paradox.</value>
            [JsonProperty("paradox")]
            public string Paradox { get; set; }

            /// <summary>
            /// Gets or sets the steam.
            /// </summary>
            /// <value>The steam.</value>
            [JsonProperty("steam")]
            public string Steam { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
