using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SteamWebAPI2.Interfaces;

namespace Steam.Explorer
{
    public class SteamExplorer
    {
        private readonly SteamApps _steamApps;

        public SteamExplorer()
        {
            this._steamApps = new SteamApps(SteamSettings.WebApiKey);
        }

        public async Task<IReadOnlyCollection<string>> GetAppNameListAsync()
        {
            var response = await _steamApps.GetAppListAsync();
            var data = response.Data;

            var names = data.Select(x => x.Name).ToArray();
            return names;
        }

        public async Task<IReadOnlyCollection<uint>> GetAppIdListAsync()
        {
            var response = await _steamApps.GetAppListAsync();

            return response.Data.Select(x => x.AppId).ToArray();
        }

        public async Task<IDictionary<uint, string>> GetAppListAsync()
        {
            var response = await _steamApps.GetAppListAsync();

            return response.Data.ToDictionary(x => x.AppId, x => x.Name);
        }

        public async Task<string> GetGameById(uint appId, ulong userSteamId)
        {
            var steam = new SteamUserStats(SteamSettings.WebApiKey);

            var user = new SteamUser(SteamSettings.WebApiKey);
            var summary = await user.GetPlayerSummaryAsync(userSteamId);

            var stats = await steam.GetUserStatsForGameAsync(userSteamId, appId);
            var name = stats.Data.GameName;

            return name;
        }
    }
}
