// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-24-2020
//
// Last Modified By : Mario
// Last Modified On : 09-07-2020
// ***********************************************************************
// <copyright file="ModCompareSelectorControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ModCompareSelectorControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModCompareSelectorControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModCompareSelectorControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModCompareSelectorControlView : BaseControl<ModCompareSelectorControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The mod compare class
        /// </summary>
        private const string ModCompareClass = "ModCompare";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCompareSelectorControlView" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ModCompareSelectorControlView(ILogger logger)
        {
            this.logger = logger;
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            void appendClass(ListBox listBox)
            {
                var children = listBox.GetLogicalChildren().Cast<ListBoxItem>();
                foreach (var item in children)
                {
                    if (!item.Classes.Contains(ModCompareClass))
                    {
                        item.Classes.Add(ModCompareClass);
                    }
                }
            }
            var left = this.FindControl<ListBox>("leftSide");
            var right = this.FindControl<ListBox>("rightSide");
            LayoutUpdated += (sender, args) =>
            {
                try
                {
                    left.InvalidateArrange();
                    right.InvalidateArrange();
                    appendClass(left);
                    appendClass(right);
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex);
                }
            };
            base.OnActivated(disposables);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion Methods
    }
}
