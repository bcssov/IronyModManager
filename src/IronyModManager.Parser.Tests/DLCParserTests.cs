// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="DLCParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Parser.DLC;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ModParserTests.
    /// </summary>
    public class DLCParserTests
    {
        /// <summary>
        /// Defines the test method Should_parse_mod_file.
        /// </summary>
        [Fact]
        public void Should_parse_dlc_file()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"name = ""Horizon Signal""");
            sb.AppendLine(@"localizable_name = ""DLC_HORIZON_SIGNAL""");
            sb.AppendLine(@"archive = ""dlc/dlc013_horizon_signal/dlc013.zip""");
            sb.AppendLine(@"steam_id = 554350");
            sb.AppendLine(@"rail_id = 2000069");
            sb.AppendLine(@"pops_id = ""horizon_signal""");
            sb.AppendLine(@"affects_checksum = no");
            sb.AppendLine(@"affects_compatability = yes");
            sb.AppendLine(@"checksum=""5b1c637b013c6ef8b5fc6bae8778b0c6""");
            sb.AppendLine(@"third_party_content = no");
            sb.AppendLine(@"category=""content_pack""");


            var parser = new DLCParser(new CodeParser(new Logger()));
            var result = parser.Parse("dlc\\dlc01.dlc", sb.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), Common.DescriptorModType.DescriptorMod);
            result.Name.Should().Be("Horizon Signal");
            result.Path.Should().Be("dlc/dlc01.dlc");
            result.AppId.Should().BeNullOrEmpty();
            result.IsVisible.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_parse_json_dlc_file.
        /// </summary>
        [Fact]
        public void Should_parse_json_dlc_file()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"{");
            sb.AppendLine(@"	""id"":");
            sb.AppendLine(@"	{");
            sb.AppendLine(@"		""steam"": ""2071471"",");
            sb.AppendLine(@"		""paradox"": ""dlc002_american_buildings""");
            sb.AppendLine(@"	},");
            sb.AppendLine(@"	""displayName"":");
            sb.AppendLine(@"	{");
            sb.AppendLine(@"		""en"": ""American Buildings Pack""");
            sb.AppendLine(@"	},");
            sb.AppendLine(@"	""category"":");
            sb.AppendLine(@"	{");
            sb.AppendLine(@"		""en"": ""DLC""");
            sb.AppendLine(@"	},");
            sb.AppendLine(@"	""description"":");
            sb.AppendLine(@"	{");
            sb.AppendLine(@"		""en"": ""Victoria 3: American Buildings Pack""");
            sb.AppendLine(@"	},");
            sb.AppendLine(@"	""thumbnailPath"": ""thumbnail.png"",");
            sb.AppendLine(@"	""thirdPartyContent"": false");
            sb.AppendLine(@"}");


            var parser = new DLCParser(new CodeParser(new Logger()));
            var result = parser.Parse("dlc\\dlc01.dlc", sb.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), Common.DescriptorModType.JsonMetadata);
            result.Name.Should().Be("American Buildings Pack");
            result.Path.Should().Be("dlc/dlc01.dlc");
            result.AppId.Should().Be("dlc002_american_buildings");
            result.IsVisible.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_parse_json_dlc_v2_file.
        /// </summary>
        [Fact]
        public void Should_parse_json_dlc_v2_file()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"{");
            sb.AppendLine(@"	""name"":	""Sacred Sites"",");
            sb.AppendLine(@"	""localizable_name"":	""D017_sacred_sites_pack"",");
            sb.AppendLine(@"	""path"":	""dlc/D017_sacred_sites_pack"",");
            sb.AppendLine(@"	""picture"":	""thumbnail.dds"",");
            sb.AppendLine(@"	""checksum"":	""e355e9e3f07e885d5c02159859aec9c4"",");
            sb.AppendLine(@"	""description"":	"""",");
            sb.AppendLine(@"	""supported_version"":	"""",");
            sb.AppendLine(@"	""dependencies"":	[],");
            sb.AppendLine(@"	""replace_path"":	[],");
            sb.AppendLine(@"	""tags"":	[],");
            sb.AppendLine(@"	""pops_id"":	""D017_sacred_sites_pack"",");
            sb.AppendLine(@"	""msgr_id"":	"""",");
            sb.AppendLine(@"	""steam_id"":	3865300,");
            sb.AppendLine(@"	""review_steam_id"":	4091680,");
            sb.AppendLine(@"	""affects_save_compatibility"":	false,");
            sb.AppendLine(@"	""mp_synced"":	false,");
            sb.AppendLine(@"	""third_party_content"":	false,");
            sb.AppendLine(@"	""enabled"":	true,");
            sb.AppendLine(@"	""hidden"":	true,");
            sb.AppendLine(@"	""verify"":	true");
            sb.AppendLine(@"}");


            var parser = new DLCParser(new CodeParser(new Logger()));
            var result = parser.Parse("dlc\\dlc01.dlc", sb.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), Common.DescriptorModType.JsonMetadataV2);
            result.Name.Should().Be("Sacred Sites");
            result.Path.Should().Be("dlc/dlc01.dlc");
            result.AppId.Should().Be("Sacred Sites");
            result.IsVisible.Should().BeFalse();
        }
    }
}
