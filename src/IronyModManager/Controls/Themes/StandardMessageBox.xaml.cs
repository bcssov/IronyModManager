// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2021
// ***********************************************************************
// <copyright file="StandardMessageBox.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Views;

namespace IronyModManager.Controls.Themes
{
    /// <summary>
    /// Class StandardMessageBox.
    /// Implements the <see cref="MessageBox.Avalonia.Views.MsBoxStandardWindow" />
    /// </summary>
    /// <seealso cref="MessageBox.Avalonia.Views.MsBoxStandardWindow" />
    public class StandardMessageBox : MsBoxStandardWindow
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMessageBox" /> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="buttonEnum">The button enum.</param>
        public StandardMessageBox(Style style, ButtonEnum buttonEnum)
        {
            SetStyle(style);
            InitializeComponent();
            SetCaptions(buttonEnum);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMessageBox" /> class.
        /// </summary>
        public StandardMessageBox()
        {
            InitializeComponent();
            SetCaptions(ButtonEnum.YesNo);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Sets the captions.
        /// </summary>
        /// <param name="buttonEnum">The button enum.</param>
        private void SetCaptions(ButtonEnum buttonEnum)
        {
            var yes = this.FindControl<Button>("yes");
            var no = this.FindControl<Button>("no");
            var ok = this.FindControl<Button>("ok");
            var cancel = this.FindControl<Button>("cancel");
            var locManager = DIResolver.Get<ILocalizationManager>();
            yes.Content = locManager.GetResource(LocalizationResources.Prompt.Yes);
            no.Content = locManager.GetResource(LocalizationResources.Prompt.No);
            if (buttonEnum == ButtonEnum.Ok)
            {
                ok.Content = locManager.GetResource(LocalizationResources.Prompt.OK);
            }
            else
            {
                ok.Content = locManager.GetResource(LocalizationResources.Prompt.Confirm);
            }
            cancel.Content = locManager.GetResource(LocalizationResources.Prompt.Cancel);
        }

        /// <summary>
        /// Sets the style.
        /// </summary>
        /// <param name="style">The style.</param>
        private void SetStyle(Style style)
        {
            var styles = Styles;
            switch (style)
            {
                case Style.Windows:
                    {
                        styles.Add(new StyleInclude(
                                new Uri("avares://MessageBox.Avalonia/Styles/Windows/Windows.xaml"))
                        { Source = new Uri("avares://MessageBox.Avalonia/Styles/Windows/Windows.xaml") });
                        break;
                    }

                case Style.MacOs:
                    {
                        styles.Add(new StyleInclude(new Uri("avares://MessageBox.Avalonia/Styles/MacOs/MacOs.xaml"))
                        { Source = new Uri("avares://MessageBox.Avalonia/Styles/MacOs/MacOs.xaml") });
                        break;
                    }

                case Style.UbuntuLinux:
                    {
                        styles.Add(new StyleInclude(
                                new Uri("avares://MessageBox.Avalonia/Styles/Ubuntu/Ubuntu.xaml"))
                        { Source = new Uri("avares://MessageBox.Avalonia/Styles/Ubuntu/Ubuntu.xaml") });
                        break;
                    }
                case Style.MintLinux:
                    {
                        styles.Add(new StyleInclude(new Uri("avares://MessageBox.Avalonia/Styles/Mint/Mint.xaml"))
                        { Source = new Uri("avares://MessageBox.Avalonia/Styles/Mint/Mint.xaml") });
                        break;
                    }
                case Style.DarkMode:
                    {
                        styles.Add(new StyleInclude(new Uri("avares://MessageBox.Avalonia/Styles/Dark/Dark.xaml"))
                        { Source = new Uri("avares://MessageBox.Avalonia/Styles/Dark/Dark.xaml") });
                        break;
                    }
                case Style.RoundButtons:
                    {
                        styles.Add(new StyleInclude(
                                new Uri("avares://MessageBox.Avalonia/Styles/RoundButtons/RoundButtons.xaml"))
                        { Source = new Uri("avares://MessageBox.Avalonia/Styles/RoundButtons/RoundButtons.xaml") });
                        break;
                    }
            }
        }

        #endregion Methods
    }
}
