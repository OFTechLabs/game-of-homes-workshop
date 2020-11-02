using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.LayoutLib;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;

namespace Blazor_Dashboard.Pages
{
    public partial class Index : ComponentBase
    {
	    Dictionary<string, List<double>> _results;
        double _totalScore;
        string _scoreDecomposition;
        List<object> _years;

        PlotlyChart scoresChart;
        Config scoresConfig = new Config();
        Layout scoresLayout = new Layout();
        // Using of the interface IList is important for the event callback!
        IList<ITrace> scoresData;

        PlotlyChart totalScoreChart;
        Config totalScoreConfig = new Config();

        Layout totalScoreLayout = new Layout()
        {
            ShowLegend = false,
            Colorway = new List<object> { "#182844" }
        };
        // Using of the interface IList is important for the event callback!
        IList<ITrace> totalScoreData;

        PlotlyChart bankruptChart;

        Config bankruptConfig = new Config()
        {
            Responsive = true
        };

        Layout bankruptLayout = new Layout()
        {
            BarMode = BarModeEnum.Stack
        };
        // Using of the interface IList is important for the event callback!
        IList<ITrace> bankruptData;

        [Inject]
        private HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var jsonString = await HttpClient.GetStringAsync("results.json");
                _results = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonString);
                var horizon = _results.First().Value.Count / 100;
                Console.WriteLine($"Horizon = {horizon}");

                var numberOfScenarios = _results["Scores"].Count / horizon;

                _totalScore = Average(_results["Scores"], horizon - 1, horizon);
                var numberOfHouses = Average(_results["NumberOfHousesScore"], horizon - 1, horizon);
                var sustainability = Average(_results["SustainabilityScores"], horizon - 1, horizon);
                var rent = Average(_results["RentScores"], horizon - 1, horizon);
                _scoreDecomposition = $"{numberOfHouses:N0},{sustainability:N0},{rent:N0}";

                var averageScore = GetAverageResult("Scores", horizon);
                var averageNumberOfHousesScore = GetAverageResult("NumberOfHousesScore", horizon);
                var averageRentScore = GetAverageResult("RentScores", horizon);
                var averageSustainabilityScore = GetAverageResult("SustainabilityScores", horizon);
                _years = Enumerable.Range(0, horizon).Select(t => DateTime.Today.Year + t).Cast<object>().ToList();
                Console.WriteLine($"Years = {_years}");

                LoadScoresData(averageScore, averageNumberOfHousesScore, averageRentScore, averageSustainabilityScore);
                LoadTotalScoreData(horizon, numberOfScenarios);
                LoadBankruptcyData(horizon);

                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        private void LoadScoresData(List<object> averageScore, List<object> averageNumberOfHousesScore, List<object> averageRentScore,
            List<object> averageSustainabilityScore)
        {
            scoresData = new List<ITrace>();

            AddScatterTrace(scoresData, averageScore, "Average total score");
            AddScatterTrace(scoresData, averageNumberOfHousesScore, "Average number of houses score");
            AddScatterTrace(scoresData, averageRentScore, "Average rent score");
            AddScatterTrace(scoresData, averageSustainabilityScore, "Average sustainability score");
        }

        private void LoadTotalScoreData(int horizon, int numberOfScenarios)
        {
            totalScoreData = new List<ITrace>();

            var rawData = _results["Scores"];

            for (var s = 0; s < numberOfScenarios; s++)
            {
                var scenario = rawData.Skip(s * horizon).Take(horizon).Cast<object>().ToList();
                AddScatterTrace(totalScoreData, scenario, $"Scenario {s + 1}");
            }
        }

        private void LoadBankruptcyData(int horizon)
        {
            bankruptData = new List<ITrace>();
            var data = _results["NumberOfBankruptcies"].Cast<object>().ToList();
            AddScatterBar(bankruptData, data, "Bankruptcies");
        }

        private List<object> GetAverageResult(string key, int horizon)
        {
            //return string.Join(',', Enumerable.Range(0, horizon).Select(t => $"{Average(_results[key], t, horizon)}"));
            return Enumerable.Range(0, horizon).Select(t => Average(_results[key], t, horizon)).Cast<object>().ToList();
        }

        private static double Average(IEnumerable<double> values, int year, int horizon)
        {
            return Math.Round(values.Where((v, s) => (s - year) % horizon == 0).Average(), 0);
        }

        private void AddScatterTrace(IList<ITrace> data, IList<object> series, string name)
        {
            data.Add(new Scatter
            {
                Name = name,
                Mode = ModeFlag.Lines | ModeFlag.Markers,
                X = _years,
                Y = series
            });
        }

        private void AddScatterBar(IList<ITrace> data, IList<object> series, string name)
        {
            data.Add(new Bar
            {
                Name = name,
                X = _years,
                Y = series
            });
        }
    }
}
