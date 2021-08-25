using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Services
{
    public class ValidateParserService
    {
        private readonly IValidateParser validateParser;

        public ValidateParserService(IValidateParser validateParser)
        {
            this.validateParser = validateParser;
        }
    }
}
