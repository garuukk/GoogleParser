using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CodeKicker.BBCode;

namespace Svyaznoy.Core.Web.Content
{
    public static class ContentHelper
    {
        private static readonly BBCodeParser Parser = new BBCodeParser(new[]
        {
            new BBTag("b", "<b>", "</b>"),
            new BBTag("p", "<p>", "</p>"),
            new BBTag("i", "<i>", "</i>"),
            new BBTag("list", "<ul>", "</ul>"),
            new BBTag("*", "<li>", "</li>", true, false),
            new BBTag("img", "<img src=\"${content}\" width=\"${width}\" height=\"${height}\" alt=\"${alt}\" />", "", false, true,
                new BBAttribute("width", "width"),
                new BBAttribute("height", "height"),
                new BBAttribute("alt", "alt")),
            new BBTag("url", "<a href=\"${href}\">", "</a>",
                new BBAttribute("href", "href"))
        });

        public static MvcHtmlString BBCodeToHtml(this HtmlHelper helper, string content)
        {
            return new MvcHtmlString(Parser.ToHtml(content));
        }
    }
}