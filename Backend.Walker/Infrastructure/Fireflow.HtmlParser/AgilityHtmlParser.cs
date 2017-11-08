using Fireflow.HtmlParser.Core;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fireflow.HtmlParser
{
    public class AgilityHtmlParser : IHtmlParser
    {
        public AgilityHtmlParser()
        {
        }

        public IList<string> GetImageSourceUrls(string source, string ext)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var htmlNods = doc.DocumentNode.SelectNodes("//img[@src]");

            var imgs = new List<string>();
            foreach (var img in htmlNods)
            {
                if (img.Attributes["src"] == null) continue;

                var src = img.Attributes["src"];
                var fileExt = Path.GetExtension(src.Value);

                if (fileExt == ext)
                    imgs.Add(src.Value);
            }

            return imgs;
        }
    }
}
