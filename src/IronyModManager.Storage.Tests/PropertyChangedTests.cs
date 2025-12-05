// ***********************************************************************
// Assembly         : IronyModManager.Storage.Tests
// Author           : Mario
// Created          : 01-29-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2020
// ***********************************************************************
// <copyright file="PropertyChangedTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Configuration;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using SimpleInjector;
using Xunit;

namespace IronyModManager.Storage.Tests
{
    /// <summary>
    /// Class PropertyChangedTests.
    /// </summary>
    public class PropertyChangedTests
    {
        private class DummyConfig : IDomainConfiguration
        {
            public DomainConfigurationOptions GetOptions()
            {
                var opts =  new DomainConfigurationOptions();
                opts.App.StoragePath = ".";
                return opts;
            }
        }

        /// <summary>
        /// Defines the test method Database_should_trigger_single_changed_event.
        /// </summary>
        [Fact]
        public void Database_should_trigger_single_changed_event()
        {
            DISetup.SetupContainer();
            var config = new Mock<IDomainConfiguration>();
            config.Setup(s => s.GetOptions()).Returns(new DummyConfig().GetOptions);
            DISetup.Container.RegisterInstance(config.Object);
            var events = new List<string>();
            var db = DIResolver.Get<IDatabase>();
            db.PropertyChanged += (s, e) =>
                {
                    events.Add(e.PropertyName);
                };
            db.Preferences = new Preferences();
            db.Preferences = db.Preferences;
            events.Count.Should().Be(1);
            events.First().Should().Be(nameof(db.Preferences));
        }


        /// <summary>
        /// Defines the test method Database_should_trigger_single_changing_event.
        /// </summary>
        [Fact]
        public void Database_should_trigger_single_changing_event()
        {
            DISetup.SetupContainer();
            var config = new Mock<IDomainConfiguration>();
            config.Setup(s => s.GetOptions()).Returns(new DummyConfig().GetOptions);
            DISetup.Container.RegisterInstance(config.Object);
            var events = new List<string>();
            var db = DIResolver.Get<IDatabase>();
            db.PropertyChanging += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            db.Preferences = new Preferences();
            db.Preferences = db.Preferences;
            events.Count.Should().Be(1);
            events.First().Should().Be(nameof(db.Preferences));
        }

        /// <summary>
        /// Defines the test method Database_should_trigger_double_changed_event.
        /// </summary>
        [Fact]
        public void Database_should_trigger_double_changed_event()
        {
            DISetup.SetupContainer();
            var config = new Mock<IDomainConfiguration>();
            config.Setup(s => s.GetOptions()).Returns(new DummyConfig().GetOptions);
            DISetup.Container.RegisterInstance(config.Object);
            var events = new List<string>();
            var db = DIResolver.Get<IDatabase>();
            db.PropertyChanged += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            db.Preferences = new Preferences();
            db.WindowState = new WindowState();
            events.Count.Should().Be(2);
            events.First().Should().Be(nameof(db.Preferences));
            events.Last().Should().Be(nameof(db.WindowState));
        }

        /// <summary>
        /// Defines the test method Database_should_trigger_double_changing_event.
        /// </summary>
        [Fact]
        public void Database_should_trigger_double_changing_event()
        {
            DISetup.SetupContainer();
            var config = new Mock<IDomainConfiguration>();
            config.Setup(s => s.GetOptions()).Returns(new DummyConfig().GetOptions);
            DISetup.Container.RegisterInstance(config.Object);
            var events = new List<string>();
            var db = DIResolver.Get<IDatabase>();
            db.PropertyChanging += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            db.Preferences = new Preferences();
            db.WindowState = new WindowState();
            events.Count.Should().Be(2);
            events.First().Should().Be(nameof(db.Preferences));
            events.Last().Should().Be(nameof(db.WindowState));
        }
    }
}
