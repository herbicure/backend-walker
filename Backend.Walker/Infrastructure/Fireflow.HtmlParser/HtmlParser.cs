using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Fireflow.HtmlParser.Core;

namespace Fireflow.HtmlParser
{
    public class HtmlParser : IHtmlParser
    {
        private readonly Regex regex = new Regex("<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public IList<string> GetImageSourceUrls(string source, string ext)
        {
            var matchedImageSources = regex.Matches(source);

            return (from Match m in matchedImageSources select m.Groups[1].Value).ToList();
        }

        public IList<string> GetAnchorReferences(string source, string ext)
        {
            var pattern = $"<a.+?href=[\"'](.+?{ext})[\"']>";
            var options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

            var matched = Regex.Matches(source, pattern, options);

            return (from Match m in matched select m.Groups[1].Value).ToList();
        }
    }
}