using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Fireflow.HtmlParser;
using Fireflow.HtmlParser.Core;
using System.Net.Http;

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
            _htmlParser = htmlParser ?? throw new ArgumentNullException(nameof(htmlParser));
        }

        public PhotosController()
            : this(new AgilityHtmlParser())
        {
        }

        #endregion

        public async Task<IList<string>> GetPhotos(string sourceUrl)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(sourceUrl);
                var htmlText = await response.Content.ReadAsStringAsync();

                var images = _htmlParser.GetImageSourceUrls(htmlText, ".jpg");

                return images;
            }
        }
    }
}