// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-16-2020
// ***********************************************************************
// <copyright file="ManagedDialog.xaml.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileChooser. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Controls.Dialogs;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Themes
{
    /// <summary>
    /// Class ManagedDialog.
    /// Implements the <see cref="Avalonia.Controls.UserControl" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.UserControl" />
    [ExcludeFromCoverage("External logic.")]
    public class ManagedDialog : UserControl
    {
        #region Fields

        /// <summary>
        /// The files view
        /// </summary>
        private readonly ListBox _filesView;

        /// <summary>
        /// The quick links root
        /// </summary>
        private readonly Control _quickLinksRoot;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedDialog" /> class.
        /// </summary>
        public ManagedDialog()
        {
            AvaloniaXamlLoader.Load(this);
            AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            _quickLinksRoot = this.FindControl<Control>("QuickLinks");
            _filesView = this.FindControl<ListBox>("Files");
            var locManager = DIResolver.Get<ILocalizationManager>();
            var fileName = this.FindControl<TextBox>("fileName");
            fileName.Watermark = locManager.GetResource(LocalizationResources.FileDialog.FileName);
            var correctingInput = false;
            fileName.PropertyChanged += (sender, args) =>
            {
                if (args.Property != TextBox.TextProperty)
                {
                    return;
                }
                if (correctingInput)
                {
                    return;
                }
                correctingInput = true;
                async Task updateText()
                {
                    await Task.Delay(1);
                    Model.FileName = Model.FileName.GenerateValidFileName();
                    correctingInput = false;
                }
                updateText().ConfigureAwait(false);
            };
            var showHiddenFiles = this.FindControl<TextBlock>("showHiddenFiles");
            showHiddenFiles.Text = locManager.GetResource(LocalizationResources.FileDialog.ShowHiddenFiles);
            var ok = this.FindControl<Button>("ok");
            ok.Content = locManager.GetResource(LocalizationResources.FileDialog.OK);
            var cancel = this.FindControl<Button>("cancel");
            cancel.Content = locManager.GetResource(LocalizationResources.FileDialog.Cancel);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        private ManagedDialogViewModel Model => DataContext as ManagedDialogViewModel;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:DataContextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override async void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            var model = (DataContext as ManagedDialogViewModel);

            if (model == null)
            {
                return;
            }

            var preselected = model.SelectedItems.FirstOrDefault();

            if (preselected == null)
            {
                return;
            }

            //Let everything to settle down and scroll to selected item
            await Task.Delay(100);

            if (preselected != model.SelectedItems.FirstOrDefault())
            {
                return;
            }

            // Workaround for ListBox bug, scroll to the previous file
            var indexOfPreselected = model.Items.IndexOf(preselected);

            if (indexOfPreselected > 1)
            {
                _filesView.ScrollIntoView(model.Items[indexOfPreselected - 1]);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!((e.Source as StyledElement)?.DataContext is ManagedDialogItemViewModel model))
            {
                return;
            }

            var isQuickLink = _quickLinksRoot.IsLogicalParentOf(e.Source as Control);
#pragma warning disable CS0618 // Type or member is obsolete
            // Yes, use doubletapped event... if only it would work properly.
            if (e.ClickCount == 2 || isQuickLink)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                if (model.ItemType == ManagedFileChooserItemType.File)
                {
                    Model?.SelectSingleFile(model);
                }
                else
                {
                    Model?.Navigate(model.Path);
                }

                e.Handled = true;
            }
        }

        #endregion Methods
    }
}
