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
    }
}
