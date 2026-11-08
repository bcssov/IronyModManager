// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 09-15-2026
//
// Last Modified By : Mario
// Last Modified On : 09-15-2026
// ***********************************************************************
// <copyright file="MacUpdatePolicyTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using AwesomeAssertions;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Tests platform update configuration policy.
    /// </summary>
    public class MacUpdatePolicyTests
    {
        /// <summary>
        /// The base policy permits both checks and installation.
        /// </summary>
        [Fact]
        public void Base_policy_should_allow_update_checks_and_installation()
        {
            var settings = Load("appSettings.json");

            settings["Updates"]!["Disable"]!.GetValue<bool>().Should().BeFalse();
            settings["Updates"]!["DisableInstallOnly"]!.GetValue<bool>().Should().BeFalse();
        }

        /// <summary>
        /// macOS keeps checking enabled while disabling only self-installation.
        /// </summary>
        [Fact]
        public void Mac_policy_should_disable_installation_only()
        {
            var replacements = GetUpdateReplacements("appSettings.osx-x64.json");

            replacements.Should().ContainSingle();
            replacements.Single()["@jdt.path"]!.GetValue<string>().Should().Be("$.DisableInstallOnly");
            replacements.Single()["@jdt.value"]!.GetValue<bool>().Should().BeTrue();
            replacements.Should().NotContain(p => p["@jdt.path"]!.GetValue<string>() == "$.Disable");
        }

        /// <summary>
        /// Windows and Linux retain the base update behavior.
        /// </summary>
        /// <param name="fileName">The platform override filename.</param>
        [Theory]
        [InlineData("appSettings.win-x64.json")]
        [InlineData("appSettings.linux-x64.json")]
        public void Other_platforms_should_not_override_update_policy(string fileName)
        {
            GetUpdateReplacements(fileName).Should().BeEmpty();
        }

        private static IReadOnlyList<JsonObject> GetUpdateReplacements(string fileName)
        {
            var updates = Load(fileName)["Updates"] as JsonObject;
            if (updates == null || updates["@jdt.replace"] == null)
            {
                return Array.Empty<JsonObject>();
            }

            var replace = updates["@jdt.replace"]!;
            return replace is JsonArray array ? array.Select(p => p!.AsObject()).ToArray() : [replace.AsObject()];
        }

        private static JsonObject Load(string fileName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "UpdatePolicy", fileName);
            return JsonNode.Parse(File.ReadAllText(path))!.AsObject();
        }
    }
}
