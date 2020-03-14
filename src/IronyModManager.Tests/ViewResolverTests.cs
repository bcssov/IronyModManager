// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="ViewResolverTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Localization;
using IronyModManager.Tests.Common;
using IronyModManager.Tests.Views;
using IronyModManager.Tests.ViewModels;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class ViewResolverTests.
    /// </summary>
    public class ViewResolverTests
    {
        /// <summary>
        /// Defines the test method Should_format_user_control_name_without_proxy.
        /// </summary>
        [Fact]
        public void Should_format_user_control_name_without_proxy()
        {
            DISetup.SetupContainer();
            var resolver = new ViewResolver();
            var name = resolver.FormatUserControlName(new FakeControlViewModel());
            name.Should().Be("IronyModManager.Tests.Views.FakeControlView");
        }

        /// <summary>
        /// Defines the test method Should_format_user_control_name_with_proxy.
        /// </summary>
        [Fact]
        public void Should_format_user_control_name_with_proxy()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeControlViewModel>();
            var instance = DISetup.Container.GetInstance<FakeControlViewModel>();
            var resolver = new ViewResolver();
            var name = resolver.FormatUserControlName(instance);
            name.Should().Be("IronyModManager.Tests.Views.FakeControlView");
        }

        /// <summary>
        /// Defines the test method Should_return_view_model_name.
        /// </summary>
        [Fact]
        public void Should_return_view_model_name()
        {
            var resolver = new ViewResolver();
            var result = resolver.FormatViewModelName<FakeControlView>();
            result.Should().Be("IronyModManager.Tests.ViewModels.FakeControlViewModel");
        }

        /// <summary>
        /// Defines the test method Should_be_control.
        /// </summary>
        [Fact]
        public void Should_be_control()
        {
            var resolver = new ViewResolver();
            resolver.IsControl("FakeControlView").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_be_control.
        /// </summary>
        [Fact]
        public void Should_not_be_control()
        {
            var resolver = new ViewResolver();
            resolver.IsControl("FakeControlViewModel").Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_return_user_control.
        /// </summary>
        [Fact]
        public void Should_return_user_control()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeControlViewModel>();
            DISetup.Container.Register<FakeControlView>();
            var resolver = new ViewResolver();
            var result = resolver.ResolveUserControl(new FakeControlViewModel());
            result.GetType().Should().Be(typeof(FakeControlView));
        }

        /// <summary>
        /// Defines the test method Should_not_return_user_control.
        /// </summary>
        [Fact]
        public void Should_not_return_user_control()
        {
            DISetup.SetupContainer();
            var resolver = new ViewResolver();
            Exception ex = null;
            try
            {
                resolver.ResolveUserControl(new NonExistingViewModel());
            }
            catch (Exception e)
            {
                ex = e;                
            }
            ex.Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_view_model.
        /// </summary>
        [Fact]
        public void Should_return_view_model() 
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeWindowViewModel>();
            DISetup.Container.Register<FakeWindow>();
            var resolver = new ViewResolver();
            var result = resolver.ResolveViewModel<FakeWindow>();
            typeof(FakeWindowViewModel).IsAssignableFrom(result.GetType()).Should().BeTrue();            
        }

        /// <summary>
        /// Defines the test method Should_not_return_view_model.
        /// </summary>
        [Fact]
        public void Should_not_return_view_model()
        {
            DISetup.SetupContainer();
            var resolver = new ViewResolver();
            Exception ex = null;
            try
            {
                resolver.ResolveViewModel<NotExistingWindow>();
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.Should().NotBeNull();
        }
    }
}
