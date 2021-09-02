using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Common.Parsers
{
    public interface IValidateParser
    {
        public IEnumerable<IDefinition> Validate(ParserArgs args);

        public IBracketValidateResult GetBracketCount(string text);
    }

    public interface IBracketValidateResult
    {
        public int OpenBracketCount { get; set; }
        public int CloseBracketCount { get; set; }
    }
}
