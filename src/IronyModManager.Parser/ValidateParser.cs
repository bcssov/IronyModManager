using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared.Models;
using IronyModManager.Shared;

namespace IronyModManager.Parser
{
    public class ValidateParser : BaseParser, IValidateParser
    {
        public ValidateParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        public override string ParserName => nameof(ValidateParser);

        public override bool CanParse(CanParseArgs args)
        {
            //this is incredibly fishy, but its what default parser has....
            return true;
        }

        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            return ParseRoot(args);
        }

        public IEnumerable<IDefinition> Validate(ParserArgs args)
        {
            return EvalForErrorsOnly(args);
        }
    }
}
