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
            //apparently it was supposed to be false, means it can be used as a parser
            return false;
        }

        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            return ParseRoot(args);
        }

        public IEnumerable<IDefinition> Validate(ParserArgs args)
        {
            return EvalForErrorsOnly(args);
        }

        public IBracketValidateResult GetBracketCount(string text)
        {
            BracketValidateResult bracketCount = new BracketValidateResult();
            var cleantext = CleanComments(text);
            

            bracketCount.CloseBracketCount = cleantext.Count(s => s == Common.Constants.Scripts.CloseObject);
            bracketCount.OpenBracketCount = cleantext.Count(s => s == Common.Constants.Scripts.OpenObject);

            return bracketCount;
        }

        /// <summary>
        /// Cleans the comments.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        /// yoink?
        protected string CleanComments(string text)
        {
            var lines = text.SplitOnNewLine();
            List<string> cleanlines = new List<string>();
            foreach (var line in lines)
            {
                if (line.IndexOf(Common.Constants.Scripts.ScriptCommentId) > 0)
                {
                    var sb = new StringBuilder();
                    var split = line.Split(Common.Constants.Scripts.ScriptCommentId);
                    var counter = 0;
                    var count = split.Length;
                    var quoteCount = 0;
                    foreach (var item in split)
                    {
                        counter++;
                        var previousQuoteCount = quoteCount;
                        quoteCount += item.Count(p => p == Common.Constants.Scripts.Quote);
                        if (counter == 1)
                        {
                            sb.Append(item);
                        }
                        else
                        {
                            var quoteIndex = item.IndexOf(Common.Constants.Scripts.Quote);
                            if (quoteIndex > -1 && quoteCount == 2 && previousQuoteCount > 0)
                            {
                                sb.Append($"#{item.Substring(0, quoteIndex + 1)}");
                                break;
                            }
                            else if (counter < count && previousQuoteCount > 0)
                            {
                                sb.Append($"#{item}");
                            }
                        }
                    }
                    cleanlines.Add(sb.ToString().Trim(Common.Constants.Scripts.ScriptCommentId));
                }
                cleanlines.Add(line);
            }
            var cleantext = String.Join("\r\n", cleanlines);
            return cleantext;
        }
    }

    public class BracketValidateResult : IBracketValidateResult
    {
        public int OpenBracketCount { get; set; }
        public int CloseBracketCount { get; set; }
    }
}
