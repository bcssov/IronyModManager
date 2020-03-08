// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-08-2020
// ***********************************************************************
// <copyright file="SortOrderControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Localization;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class SortOrderControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class SortOrderControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortOrderControlViewModel" /> class.
        /// </summary>
        /// <param name="localizationManager">The localization manager.</param>
        public SortOrderControlViewModel(ILocalizationManager localizationManager)
        {
            this.localizationManager = localizationManager;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the sort command.
        /// </summary>
        /// <value>The sort command.</value>
        public virtual ReactiveCommand<Unit, Unit> SortCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public virtual SortOrder SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the sort order text.
        /// </summary>
        /// <value>The sort order text.</value>
        public virtual string SortOrderText { get; protected set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public virtual string Text { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the sort order.
        /// </summary>
        /// <param name="order">The order.</param>
        public void SetSortOrder(SortOrder order)
        {
            SortOrder = order;
            MapSortOrder(order);
        }

        /// <summary>
        /// Maps the sort order.
        /// </summary>
        /// <param name="order">The order.</param>
        protected virtual void MapSortOrder(SortOrder order)
        {
            var text = string.Empty;
            switch (order)
            {
                case SortOrder.Asc:
                    text = localizationManager.GetResource(LocalizationResources.Sorting.Sort_A_Z);
                    break;

                case SortOrder.Desc:
                    text = localizationManager.GetResource(LocalizationResources.Sorting.Sort_Z_A);
                    break;

                default:
                    break;
            }
            SortOrderText = text;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            MapSortOrder(SortOrder);

            SortCommand = ReactiveCommand.Create(() =>
            {
                if (SortOrder == SortOrder.Asc)
                {
                    SortOrder = SortOrder.Desc;
                }
                else
                {
                    SortOrder = SortOrder.Asc;
                }
                MapSortOrder(SortOrder);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
