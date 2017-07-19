using System.Collections.Generic;

namespace Fireflow.HtmlParser.Core
{
    public interface IHtmlParser
    {
        IList<string> GetAnchorReferences(string source, string ext);
        IList<string> GetImageSourceUrls(string source);
        string GetFirstReference(string source, string ext);
    }
}
