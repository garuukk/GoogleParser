using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoogleParser.Core.Entities;
using HtmlAgilityPack;

namespace GoogleParser.Core.Parsers
{
    public class GoogleMarketParse : IMarketParser
    {
        public IEnumerable<string> ParseCollectionPage(string pageHtml)
        {
            if (string.IsNullOrEmpty(pageHtml))
                return null;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(pageHtml);

            var actionBody = FindCardNodes(html);

            var applicationPageLinks = new List<string>();

            foreach (var htmlNode in actionBody)
            {
                var a = htmlNode.Descendants("a").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("card-click-target"));

                if (a != null)
                {
                    var bodyUrl = a.Attributes["href"].Value;
                    applicationPageLinks.Add(bodyUrl);
                }
            }

            return applicationPageLinks;
        }

        public ParsedAppCard ParseAppPage(string pageHtml)
        {
            if (string.IsNullOrEmpty(pageHtml))
                return null;

            ParsedAppCard card = new ParsedAppCard();

            var html = new HtmlDocument();
            html.LoadHtml(pageHtml);

            var divName = html.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("id-app-title"));
            if (divName != null)
            {
                card.Name = HttpUtility.HtmlDecode(divName.InnerText);
            }

            var contentDiv = html.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("details-section metadata"));

            if (contentDiv == null)
                return null;

            var divMetaInfos = contentDiv.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("meta-info")).ToList();

            foreach (var divMetaInfo in divMetaInfos)
            {
                var title = divMetaInfo.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("title"));

                if (title != null)
                {
                    if (!string.IsNullOrEmpty(title.InnerText) && title.InnerText.Contains("Разработчик"))
                    {
                        var aLinks = divMetaInfo.Descendants("a").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("dev-link")).ToList();

                        foreach (var aLink in aLinks)
                        {
                            var link = aLink.Attributes["href"].Value;
                            if (link != null && link.Contains("mailto"))
                            {
                                card.DeveloperEmail = HttpUtility.HtmlDecode(link.Substring(7, link.Length - 7));
                            }
                        }

                        var divPhysicalAddress =
                            divMetaInfo.Descendants("div")
                                .FirstOrDefault(
                                    d =>
                                        d.Attributes.Contains("class") &&
                                        d.Attributes["class"].Value.Contains("content physical-address"));

                        if (divPhysicalAddress != null)
                        {
                            card.DeveloperPhysicalAddress = divPhysicalAddress.InnerText;
                        }

                    }
                    else if (!string.IsNullOrEmpty(title.InnerText) && title.InnerText.Contains("Продавец"))
                    {
                        var saler = divMetaInfo.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("content"));
                        if (saler != null)
                        {
                            card.DeveloperName = HttpUtility.HtmlDecode(saler.InnerText);
                        }
                    }
                }

            }


            return card;
        }

        private IEnumerable<HtmlNode> FindCardNodes(HtmlDocument document)
        {
            var result = document.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("card no-rationale square-cover apps small")).ToList();
            return result;
        }
    }
}
