// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="HashReport.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class HashReport.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IHashReport" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IHashReport" />
    public class HashReport : BaseModel, IHashReport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public virtual IList<IHashFileReport> Reports { get; set; } = new List<IHashFileReport>();

        /// <summary>
        /// Gets or sets the type of the report.
        /// </summary>
        /// <value>The type of the report.</value>
        public virtual HashReportType ReportType { get; set; }

        #endregion Properties
    }
}
