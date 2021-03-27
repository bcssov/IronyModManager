// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-01-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="ModHashReportControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ModHashReportControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModHashReportControlViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.FileHash.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public virtual IEnumerable<HashReport> Reports { get; protected set; }

        /// <summary>
        /// Gets or sets the selected report.
        /// </summary>
        /// <value>The selected report.</value>
        public virtual HashReport SelectedReport { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close.
        /// </summary>
        public virtual void ForceClose()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="reports">The reports.</param>
        public virtual void SetParameters(IEnumerable<IHashReport> reports)
        {
            var converted = new List<HashReport>();
            if (reports?.Count() > 0)
            {
                foreach (var item in reports)
                {
                    converted.Add(new HashReport(item));
                }
            }
            Reports = converted;
            if (converted.Count > 0)
            {
                SelectedReport = converted.FirstOrDefault();
            }
            IsOpen = true;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);

            CloseCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
            }).DisposeWith(disposables);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ModFileHashReport.
        /// </summary>
        public class FileHashReport
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="FileHashReport" /> class.
            /// </summary>
            /// <param name="item">The item.</param>
            public FileHashReport(IHashFileReport item)
            {
                File = item?.File;
                Hash = item?.Hash;
                SecondHash = item?.SecondHash;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the display.
            /// </summary>
            /// <value>The display.</value>
            public string Display
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(Hash) && !string.IsNullOrWhiteSpace(SecondHash))
                    {
                        return $"{File} ({Hash} - {SecondHash})";
                    }
                    else if (!string.IsNullOrWhiteSpace(Hash))
                    {
                        return $"{File} ({Hash})";
                    }
                    else if (!string.IsNullOrWhiteSpace(SecondHash))
                    {
                        return $"{File} ({SecondHash})";
                    }
                    else
                    {
                        return File;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File { get; set; }

            /// <summary>
            /// Gets or sets the hash.
            /// </summary>
            /// <value>The hash.</value>
            public string Hash { get; set; }

            /// <summary>
            /// Gets or sets the second hash.
            /// </summary>
            /// <value>The second hash.</value>
            public string SecondHash { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class HashReport.
        /// </summary>
        public class HashReport
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="HashReport" /> class.
            /// </summary>
            /// <param name="item">The item.</param>
            public HashReport(IHashReport item)
            {
                Name = item?.Name;
                var reports = new List<FileHashReport>();
                if (item != null && item.Reports.Any())
                {
                    foreach (var report in item.Reports)
                    {
                        reports.Add(new FileHashReport(report));
                    }
                }
                Reports = reports;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the reports.
            /// </summary>
            /// <value>The reports.</value>
            public IList<FileHashReport> Reports { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
