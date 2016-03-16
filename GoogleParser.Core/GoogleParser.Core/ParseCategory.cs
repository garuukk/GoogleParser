using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using GoogleParser.Core.Entities;
using HtmlAgilityPack;
using System.Linq;

namespace GoogleParser.Core
{
    public class ParseCategory
    {
        public IEnumerable<ParsedAppCard> Parse(string url)
        {
            var responseData = SendRequest(url);

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseData);

            var actionBody = html.DocumentNode.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("card no-rationale square-cover apps small")).ToList();

            var cards = new List<ParsedAppCard>();

            foreach (var htmlNode in actionBody)
            {
                var a = htmlNode.Descendants("a").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("card-click-target"));

                if (a != null)
                {
                    var bodyUrl = a.Attributes["href"].Value;
                    var appUrl = "https://play.google.com" + bodyUrl;

                    var response = SendRequest(appUrl);

                    var card = ParseAppCard(response);

                    if (card != null)
                    {
                        card.Url = appUrl;
                        cards.Add(card);
                    }

                    Thread.Sleep(1000);
                }
            }

            return cards;
        }

        protected ParsedAppCard ParseAppCard(string pageHtml)
        {
            ParsedAppCard card = new ParsedAppCard();

            var html = new HtmlDocument();
            html.LoadHtml(pageHtml);

            var divName = html.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("id-app-title"));
            if (divName != null)
            {
                card.Name = divName.InnerText;
            }

            var contentDiv = html.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("details-section metadata"));

            if (contentDiv == null)
                return null;

            var divMetaInfos = contentDiv.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("meta-info")).ToList();

            foreach (var  divMetaInfo in divMetaInfos)
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
                                card.DeveloperEmail = link.Substring(7, link.Length - 7);
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(title.InnerText) && title.InnerText.Contains("Продавец"))
                    {
                        var saler = divMetaInfo.Descendants("div").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("content"));
                        if (saler != null)
                        {
                            card.DeveloperName = saler.InnerText;
                        }
                    }
                }

            }


            return card;
        }

        protected string SendRequest(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = WebRequestMethods.Http.Get;

            webRequest.ServicePoint.Expect100Continue = false;

            webRequest.KeepAlive = true;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            string responseData = string.Empty;

            using (Stream stream = webRequest.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                responseData = reader.ReadToEnd();
            }

            return responseData;
        }
    }
}
