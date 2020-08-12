// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="SQLTraceLog.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared;
using Newtonsoft.Json;
using RepoDb;
using RepoDb.Interfaces;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class SQLTraceLog.
    /// Implements the <see cref="RepoDb.Interfaces.ITrace" />
    /// </summary>
    /// <seealso cref="RepoDb.Interfaces.ITrace" />
    [ExcludeFromCoverage("Skipping testing SQL logic.")]
    internal class SQLTraceLog : ITrace
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLTraceLog" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SQLTraceLog(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Afters the average.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterAverage(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the average all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterAverageAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the batch query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterBatchQuery(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the count.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterCount(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the count all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterCountAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the delete.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterDelete(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the delete all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterDeleteAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the execute non query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterExecuteNonQuery(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the execute query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterExecuteQuery(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the execute reader.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterExecuteReader(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the execute scalar.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterExecuteScalar(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the exists.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterExists(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// The AfterInsert event occurs after a new record is added.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterInsert(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the insert all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterInsertAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the maximum.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterMax(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the maximum all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterMaxAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the merge.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterMerge(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the merge all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterMergeAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the minimum.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterMin(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the minimum all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterMinAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterQuery(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the query all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterQueryAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the query multiple.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterQueryMultiple(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the sum.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterSum(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the sum all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterSumAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the truncate.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterTruncate(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the update.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterUpdate(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Afters the update all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AfterUpdateAll(TraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the average.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeAverage(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the average all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeAverageAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the batch query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeBatchQuery(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the count.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeCount(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the count all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeCountAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the delete.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeDelete(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the delete all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeDeleteAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the execute non query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeExecuteNonQuery(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the execute query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeExecuteQuery(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the execute reader.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeExecuteReader(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the execute scalar.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeExecuteScalar(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the exists.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeExists(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the insert.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeInsert(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the insert all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeInsertAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the maximum.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeMax(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the maximum all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeMaxAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the merge.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeMerge(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the merge all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeMergeAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the minimum.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeMin(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the minimum all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeMinAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the query.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeQuery(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the query all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeQueryAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the query multiple.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeQueryMultiple(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the sum.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeSum(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the sum all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeSumAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the truncate.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeTruncate(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the update.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeUpdate(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Befores the update all.
        /// </summary>
        /// <param name="log">The log.</param>
        public void BeforeUpdateAll(CancellableTraceLog log)
        {
            TraceQuery(log);
        }

        /// <summary>
        /// Traces the query.
        /// </summary>
        /// <param name="log">The log.</param>
        private void TraceQuery(TraceLog log)
        {
#if DEBUG
            var msg = JsonConvert.SerializeObject(log);
            logger.Trace(msg);
#endif
        }

        /// <summary>
        /// Traces the query.
        /// </summary>
        /// <param name="log">The log.</param>
        private void TraceQuery(CancellableTraceLog log)
        {
#if DEBUG
            var msg = JsonConvert.SerializeObject(log);
            logger.Trace(msg);
#endif
        }

        #endregion Methods
    }
}
