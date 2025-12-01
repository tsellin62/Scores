using RestSharp;
using Scores.Models;
using System.Text.Json;

namespace Scores.Services
{
    public class CfpApiService
    {
        private readonly RestClient _client = new RestClient("https://site.api.espn.com");

        public async Task<List<CfpTeam>?> GetCfpRankingsAsync()
        {
            int year = 2025;
            var teamsList = new List<CfpTeam>();

            var request = new RestRequest("/apis/site/v2/sports/football/college-football/rankings", Method.Get);

            // ⭐ REQUIRED — ESPN blocks requests without a browser UA
            request.AddHeader("User-Agent", "Mozilla/5.0");

            request.AddParameter("year", year);

            RestResponse response = await _client.ExecuteAsync(request);

            // ----------------------------
            // DEBUG LOGS — very important!
            // ----------------------------
            System.Diagnostics.Debug.WriteLine("➡️ API CALL COMPLETE");
            System.Diagnostics.Debug.WriteLine($"➡️ HTTP Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"➡️ Success? {response.IsSuccessful}");
            System.Diagnostics.Debug.WriteLine($"➡️ Content length: {response.Content?.Length ?? 0}");
            System.Diagnostics.Debug.WriteLine($"➡️ Error message: {response.ErrorMessage}");
            // ----------------------------

            if (!response.IsSuccessful || response.Content == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Request failed, returning empty team list.");
                return teamsList;
            }

            using var doc = JsonDocument.Parse(response.Content);
            var root = doc.RootElement;

            if (!root.TryGetProperty("rankings", out var rankings))
                return teamsList;

            foreach (var ranking in rankings.EnumerateArray())
            {
                if (!ranking.TryGetProperty("shortName", out var sn)) continue;
                if (sn.GetString() != "CFP Rankings") continue;

                if (!ranking.TryGetProperty("ranks", out var ranks)) continue;

                foreach (var r in ranks.EnumerateArray())
                {
                    try
                    {
                        int rank = r.GetProperty("current").GetInt32();
                        string record = r.GetProperty("recordSummary").GetString() ?? "";

                        var team = r.GetProperty("team");
                        string nickname = team.TryGetProperty("nickname", out var nn) ? nn.GetString() ?? "" : "";
                        string displayName = team.TryGetProperty("displayName", out var dn) ? dn.GetString() ?? nickname : nickname;

                        // Filter out non-FBS
                        string groupName = "";
                        if (team.TryGetProperty("groups", out var groups) &&
                            groups.TryGetProperty("parent", out var parent) &&
                            parent.TryGetProperty("shortName", out var gs))
                        {
                            groupName = gs.GetString() ?? "";
                        }

                        if (groupName != "FBS")
                            continue;

                        string logoUrl = "";
                        if (team.TryGetProperty("logos", out var logos) &&
                            logos.ValueKind == JsonValueKind.Array &&
                            logos.GetArrayLength() > 0)
                        {
                            logoUrl = logos[0].GetProperty("href").GetString() ?? "";
                        }

                        teamsList.Add(new CfpTeam
                        {
                            Rank = rank,
                            Team = displayName,
                            Record = record,
                            LogoUrl = logoUrl
                        });
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"➡️ Parsed teams: {teamsList.Count}");

            return teamsList;
        }
    }
}
