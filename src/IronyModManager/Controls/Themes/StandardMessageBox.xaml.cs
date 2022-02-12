// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="StandardMessageBox.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
        /// <param name="buttonEnum">The button enum.</param>
        public StandardMessageBox(ButtonEnum buttonEnum)
        {
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

        #endregion Methods
    }
}
