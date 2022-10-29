// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
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
using FluentAssertions;
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
        }
    }
}
