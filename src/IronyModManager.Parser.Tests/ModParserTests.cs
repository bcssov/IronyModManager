﻿// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Nick Butcher
// Last Modified On : 12-12-2021
// ***********************************************************************
// <copyright file="ModParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.Parser.Mod;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ModParserTests.
    /// </summary>
    public class ModParserTests
    {
        /// <summary>
        /// Defines the test method Should_parse_mod_file.
        /// </summary>
        [Fact]
        public void Should_parse_mod_file()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"name=""AI Species Limit""");
            sb.AppendLine(@"path=""path""");
            sb.AppendLine(@"user_dir=""dir""");
            sb.AppendLine(@"replace_path=""replace""");
            sb.AppendLine(@"tags={");
            sb.AppendLine(@"	""Gameplay""");
            sb.AppendLine(@"	""Fixes""");
            sb.AppendLine(@"}");
            sb.AppendLine(@"picture=""thumbnail.png""");
            sb.AppendLine(@"supported_version=""2.5.*""");
            sb.AppendLine(@"remote_file_id=""1830063425""");
            sb.AppendLine(@"version = ""version""");
            sb.AppendLine(@"dependencies = {");
            sb.AppendLine(@"	""fake""");
            sb.AppendLine(@"}");

            var parser = new ModParser(new CodeParser(new Logger()));
            var result = parser.Parse(sb.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            result.Dependencies.Count().Should().Be(1);
            result.Dependencies.First().Should().Be("fake");
            result.FileName.Should().Be("path");
            result.Name.Should().Be("AI Species Limit");
            result.Picture.Should().Be("thumbnail.png");
            result.RemoteId.Should().Be(1830063425);            
            result.Tags.Count().Should().Be(2);
            result.Tags.First().Should().Be("Gameplay");
            result.Tags.Last().Should().Be("Fixes");
            result.Version.Should().Be("2.5.*");
            result.UserDir.Count().Should().Be(1);
            result.UserDir.FirstOrDefault().Should().Be("dir");
            result.ReplacePath.Count().Should().Be(1);
            result.ReplacePath.FirstOrDefault().Should().Be("replace");
        }

        /// <summary>
        /// Defines the test method Should_parse_mod_file_archive.
        /// </summary>
        [Fact]
        public void Should_parse_mod_file_archive()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"name=""AI Species Limit""");
            sb.AppendLine(@"archive=""path""");
            sb.AppendLine(@"user_dir=""dir""");
            sb.AppendLine(@"replace_path=""replace""");
            sb.AppendLine(@"tags={");
            sb.AppendLine(@"	""Gameplay""");
            sb.AppendLine(@"	""Fixes""");
            sb.AppendLine(@"}");
            sb.AppendLine(@"picture=""thumbnail.png""");
            sb.AppendLine(@"supported_version=""2.5.*""");
            sb.AppendLine(@"remote_file_id=""1830063425""");
            sb.AppendLine(@"version = ""version""");
            sb.AppendLine(@"dependencies = {");
            sb.AppendLine(@"	""fake""");
            sb.AppendLine(@"}");

            var parser = new ModParser(new CodeParser(new Logger()));
            var result = parser.Parse(sb.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));            
            result.FileName.Should().Be("path");            
        }

        /// <summary>
        /// Defines the test method Should_parse_mod_file.
        /// </summary>
        [Fact]
        public void Should_parse_mod_file_with_escaped_quotations()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"name=""AI Species \""Limit\""""");
            sb.AppendLine(@"path=""path""");
            sb.AppendLine(@"user_dir=""dir""");
            sb.AppendLine(@"replace_path=""replace""");
            sb.AppendLine(@"tags={");
            sb.AppendLine(@"	""Gameplay""");
            sb.AppendLine(@"	""Fixes""");
            sb.AppendLine(@"}");
            sb.AppendLine(@"picture=""thumbnail.png""");
            sb.AppendLine(@"supported_version=""2.5.*""");
            sb.AppendLine(@"remote_file_id=""1830063425""");
            sb.AppendLine(@"version = ""version""");
            sb.AppendLine(@"dependencies = {");
            sb.AppendLine(@"	""fake""");
            sb.AppendLine(@"}");

            var parser = new ModParser(new CodeParser(new Logger()));
            var result = parser.Parse(sb.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            result.Dependencies.Count().Should().Be(1);
            result.Dependencies.First().Should().Be("fake");
            result.FileName.Should().Be("path");
            result.Name.Should().Be("AI Species \"Limit\"");
            result.Picture.Should().Be("thumbnail.png");
            result.RemoteId.Should().Be(1830063425);
            result.Tags.Count().Should().Be(2);
            result.Tags.First().Should().Be("Gameplay");
            result.Tags.Last().Should().Be("Fixes");
            result.Version.Should().Be("2.5.*");
            result.UserDir.Count().Should().Be(1);
            result.UserDir.FirstOrDefault().Should().Be("dir");
            result.ReplacePath.Count().Should().Be(1);
            result.ReplacePath.FirstOrDefault().Should().Be("replace");
        }
    }
}
