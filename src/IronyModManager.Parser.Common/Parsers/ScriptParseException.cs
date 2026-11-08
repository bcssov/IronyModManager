using System;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// An expected script-processing rejection reported by the parser.
    /// </summary>
    public sealed class ScriptParseException(IScriptError error) : Exception(error.Message)
    {
        /// <summary>
        /// Gets the original parser error, including its location when available.
        /// </summary>
        public IScriptError Error { get; } = error;
    }
}
