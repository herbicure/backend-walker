using System.Collections.Generic;

namespace Fireflow.HtmlParser.Core
{
    public interface IHtmlParser
    {
        IList<string> GetImageSourceUrls(string source, string ext);
    }
}
