using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Fireflow.HtmlParser;
using Fireflow.HtmlParser.Core;

namespace Lumberjack.PhotoManagementService.Controllers
{
    [Authorize]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class PhotosController : ApiController
    {
        private readonly IHtmlParser _htmlParser;

        #region Constructors

        public PhotosController(IHtmlParser htmlParser)
        {
            if (htmlParser == null)
                throw new ArgumentNullException(nameof(htmlParser));

            _htmlParser = htmlParser;
        }

        public PhotosController()
            : this(new HtmlParser())
        {
        }

        #endregion

        public async Task<IList<string>> GetPhotos(string sourceUrl)
        {
            var webClient = new WebClient();
            var htmlText = await webClient.DownloadStringTaskAsync(sourceUrl);

            //return _htmlParser.GetAnchorReferences(htmlText, ".jpg");
            return _htmlParser.GetImageSourceUrls(htmlText);
        }
    }
}