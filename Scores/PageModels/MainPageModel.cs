using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Scores.Models;

namespace Scores.PageModels;

public partial class MainPageModel : ObservableObject
{
    [ObservableProperty]
    private List<CfpTeam> _cfpTeams = new List<CfpTeam>();

    public MainPageModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var res in assembly.GetManifestResourceNames())
        {
            System.Diagnostics.Debug.WriteLine(res);
        }
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
