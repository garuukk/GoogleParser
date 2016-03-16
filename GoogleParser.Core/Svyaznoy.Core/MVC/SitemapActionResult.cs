using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Svyaznoy.Core.MVC
{
    public class SitemapActionResult : ActionResult
    {
        private readonly string _siteUrl;
        private readonly IEnumerable<SitemapItem> _items;

        /// <summary>
        /// site = 'http://www.site.com'
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="items"></param>
        public SitemapActionResult(string siteUrl, IEnumerable<SitemapItem> items)
        {
            if (string.IsNullOrWhiteSpace(siteUrl)) throw new ArgumentNullException("siteUrl");
            _siteUrl = siteUrl;
            this._items = items?? new List<SitemapItem>();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/xml";
            context.HttpContext.Response.ContentEncoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                writer.WriteStartElement("url");
                writer.WriteElementString("loc", _siteUrl);

                writer.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd"));

                writer.WriteElementString("changefreq", ChangeFreq.daily.ToString());
                writer.WriteElementString("priority", "1.0");
                writer.WriteEndElement();

                if (_items.HasValues())
                {
                    foreach (SitemapItem item in this._items)
                    {
                        writer.WriteStartElement("url");
                        writer.WriteElementString("loc", _siteUrl + item.Path);

                        writer.WriteElementString("lastmod", item.ChangedDate.ToString("yyyy-MM-dd"));

                        writer.WriteElementString("changefreq", item.ChangeFreq.ToString());
                        writer.WriteElementString("priority", item.Priority.ToString("G").Replace(',','.'));
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.Flush();
                writer.Close();
            }
        }
    }

    public class SitemapItem
    {
        public SitemapItem()
        {
            ChangeFreq = ChangeFreq.daily;
            Priority = 1;
        }

        public DateTime ChangedDate { get; set; }

        public ChangeFreq ChangeFreq { get; set; }

        public decimal Priority { get; set; }

        /// <summary>
        /// Should start with '/'
        /// </summary>
        public string Path { get; set; }
    }

    public enum ChangeFreq
    {
        always = 0,
        hourly = 1,
        daily = 2,
        weekly = 3,
        monthly = 4,
        yearly = 5,
        never = 6
    }
}