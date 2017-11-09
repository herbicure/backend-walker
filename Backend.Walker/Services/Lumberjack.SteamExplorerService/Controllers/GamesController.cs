using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Steam.Explorer;
using System.Web.Http.Cors;

namespace Lumberjack.SteamExplorerService.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class GamesController : ApiController
    {
        private List<uint> _gamesIds = new List<uint>();

        public GamesController()
        {
            this._gamesIds.Add(578080);
        }

        public async Task<IHttpActionResult> GetGames()
        {
            var explorer = new SteamExplorer();
            var payload = await explorer.GetAppListAsync();

            return Ok(payload);
        }

        public async Task<IHttpActionResult> GetGame(uint appId)
        {
            var explorer = new SteamExplorer();
            var payload = await explorer.GetGameById(appId, 76561198037686690);

            return Ok(payload);
        }
    }
}
