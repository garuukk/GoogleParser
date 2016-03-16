using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Svyaznoy.Core.MVC
{
    public class SitemapIndexActionResult : ActionResult
    {
        private readonly string _siteUrl;
        private readonly IEnumerable<SitemapIndexItem> _items;

        /// <summary>
        /// site = 'http://www.site.com'
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="items"></param>
        public SitemapIndexActionResult(string siteUrl, IEnumerable<SitemapIndexItem> items)
        {
            if (string.IsNullOrWhiteSpace(siteUrl)) throw new ArgumentNullException("siteUrl");
            _siteUrl = siteUrl;
            this._items = items ?? new List<SitemapIndexItem>();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            context.HttpContext.Response.ContentEncoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                writer.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

                if (_items.HasValues())
                {
                    foreach (var item in this._items)
                    {
                        writer.WriteStartElement("sitemap");
                        writer.WriteElementString("loc", _siteUrl + item.Path);
                        writer.WriteElementString("lastmod", item.ChangedDate.ToString("yyyy-MM-dd"));
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.Flush();
                writer.Close();
            }
        }
    }

    public class SitemapIndexItem
    {
        public SitemapIndexItem()
        {
        }

        public DateTime ChangedDate { get; set; }

        /// <summary>
        /// Should start with '/'
        /// </summary>
        public string Path { get; set; }
    }
}