using System.Collections.Generic;
using GoogleParser.Core.Entities;

namespace GoogleParser.Core.Parsers
{
    public interface IMarketParser
    {
        IEnumerable<string> ParseCollectionPage(string pageHtml);

        ParsedAppCard ParseAppPage(string pageHtml);
    }
}
