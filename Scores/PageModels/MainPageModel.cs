using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Scores.Models;

namespace Scores.PageModels;

public partial class MainPageModel : ObservableObject
{
    [ObservableProperty]
    private List<CfpTeam> _cfpTeams = new List<CfpTeam>();

    private readonly CfpApiService _apiService = new();

    public MainPageModel()
    {
        LoadCfpTeamsAsync();
    }

    private async void LoadCfpTeamsAsync()
    {
        try
        {
            //Fetch from ESPN API using your exact code logic
            var liveTeams = await _apiService.GetCfpRankingsAsync();

            if (liveTeams != null && liveTeams.Count > 0)
            {
                CfpTeams = liveTeams;
                System.Diagnostics.Debug.WriteLine("Loaded live CFP teams.");
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API error: {ex.Message}");
        }

        // fallback to your embedded JSON
        LoadCfpTeams();
    }


    private void LoadCfpTeams()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Scores.Resources.Raw.cfp_rankings.json");
            if (stream == null)
            {
                System.Diagnostics.Debug.WriteLine("cfp_rankings.json not found!");
                return;
            }

            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            var loadedTeams = JsonSerializer.Deserialize<List<CfpTeam>>(json);
            if (loadedTeams != null)
            {
                CfpTeams = loadedTeams;
                System.Diagnostics.Debug.WriteLine($"Loaded {CfpTeams.Count} CFP teams.");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading CFP teams: {ex.Message}");
        }
    }
}
