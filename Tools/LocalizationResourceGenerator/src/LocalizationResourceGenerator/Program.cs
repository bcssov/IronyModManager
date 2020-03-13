// ***********************************************************************
// Assembly         : LocalizationResourceGenerator
// Author           : Mario
// Created          : 02-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="Program.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LocalizationResourceGenerator
{
    /// <summary>
    /// Class Program.
    /// </summary>
    internal class Program
    {
#if DEBUG

        /// <summary>
        /// The resource path
        /// </summary>
        private const string LocalizationResourceOutPath = "..\\..\\..\\..\\..\\..\\..\\src\\IronyModManager.Shared\\LocalizationResources.cs";

        /// <summary>
        /// The localization resource path
        /// </summary>
        private const string LocalizationResourcePath = "..\\..\\..\\..\\..\\..\\..\\src\\IronyModManager\\Localization\\en.json";

#else

        #region Fields

        /// <summary>
        /// The localization resource out path
        /// </summary>
        private const string LocalizationResourceOutPath = "..\\..\\src\\IronyModManager.Shared\\LocalizationResources.cs";

        /// <summary>
        /// The localization resource path
        /// </summary>
        private const string LocalizationResourcePath = "..\\..\\src\\IronyModManager\\Localization\\en.json";

        #endregion Fields

#endif

        #region Methods

        /// <summary>
        /// Formats the indentation.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="content">The content.</param>
        /// <returns>System.String.</returns>
        private static string FormatIndentation(short level, string content)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < level * 4; i++)
            {
                sb.Append(" ");
            }
            sb.Append(content);
            return sb.ToString();
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
            short indent = 1;
            var sb = new StringBuilder();
            sb.AppendLine("// This file was automatically generated");
            sb.AppendLine("// Do not modify.");
            sb.AppendLine("// Use the LocalizationResourceGenerator instead.");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("");
            sb.AppendLine("namespace IronyModManager.Shared");
            sb.AppendLine("{");
            sb.AppendLine(FormatIndentation(indent, "public static class LocalizationResources"));
            sb.AppendLine(FormatIndentation(indent, "{"));

            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(exePath);

            var obj = JObject.Parse(File.ReadAllText(LocalizationResourcePath));

            static void ParseLocalization(StringBuilder sb, short indent, JProperty property, List<string> additionalPrefix)
            {
                var origIndent = indent;
                indent++;
                sb.AppendLine(FormatIndentation(indent, $"public static class {property.Name}"));
                sb.AppendLine(FormatIndentation(indent, "{"));

                if (property.Value.Type == JTokenType.Object)
                {
                    short propIndent = (short)(indent + 1);
                    if (additionalPrefix?.Count > 0)
                    {                        
                        sb.AppendLine(FormatIndentation(propIndent, $"public const string Prefix = \"{string.Join(".", additionalPrefix)}.\" + nameof({property.Name}) + \".\";"));
                    }
                    else
                    {
                        sb.AppendLine(FormatIndentation(propIndent, $"public const string Prefix = nameof({property.Name}) + \".\";"));
                    }
                    if (additionalPrefix == null)
                    {
                        additionalPrefix = new List<string>();
                    }
                    if (origIndent == 1)
                    {
                        additionalPrefix.Add(property.Name);
                    }                    
                    foreach (var item in property.Value.Children<JProperty>())
                    {
                        if (item.Value.Type == JTokenType.Object)
                        {
                            ParseLocalization(sb, indent, item, additionalPrefix);
                        }
                        else
                        {
                            ParseProperty(sb, propIndent, item);
                        }
                    }
                }

                sb.AppendLine(FormatIndentation(indent, "}"));
            };

            static void ParseProperty(StringBuilder sb, short indent, JProperty property)
            {
                sb.AppendLine(FormatIndentation(indent, $"public const string {property.Name} = Prefix + \"{property.Name}\";"));
            }

            foreach (var o in obj.Children<JProperty>())
            {
                ParseLocalization(sb, indent, o, null);
            }
            sb.AppendLine(FormatIndentation(indent, "}"));
            sb.AppendLine("}");
            File.WriteAllText(LocalizationResourceOutPath, sb.ToString());
        }

        #endregion Methods
    }
}
