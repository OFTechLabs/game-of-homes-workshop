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
	    Dictionary<string, List<object>> _results;
        double _totalScore;
        string _scoreDecomposition;
        List<object> _years;

        PlotlyChart scoresChart;
        Config scoresConfig = new Config()
        {
            Responsive = true
        };
        Layout scoresLayout = new Layout();
        // Using of the interface IList is important for the event callback!
        IList<ITrace> scoresData;

        PlotlyChart averageResultsChart;
        Config averageResultsConfig = new Config()
        {
            Responsive = true
        };

        Layout averageResultsLayout = new Layout()
        {
            //Colorway = new List<object> { "#182844" }
        };
        IList<ITrace> averageResultsData;

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

        PlotlyChart debtChart;

        Config debtConfig = new Config()
        {
            Responsive = true
        };

        Layout debtLayout = new Layout();
    
        IList<ITrace> debtData;

        [Inject]
        private HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var jsonString = await HttpClient.GetStringAsync("results.json");
                _results = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonString).ToDictionary(p => p.Key, p => p.Value.Cast<object>().ToList());
                var horizon = _results.Values.First().Count;
                Console.WriteLine($"Horizon = {horizon}");

                var numberOfScenarios = _results["Scores"].Count / horizon;

                _totalScore = (double)_results["Scores"][horizon - 1];
                var numberOfHouses =(double)_results["NumberOfHousesScore"][horizon - 1];
                var sustainability = (double)_results["SustainabilityScores"][horizon - 1];
                var rent = (double)_results["RentScores"][horizon - 1];
                _scoreDecomposition = $"{numberOfHouses:N0},{sustainability:N0},{rent:N0}";

                var averageScore = _results["Scores"];
                var averageNumberOfHousesScore = _results["NumberOfHousesScore"];
                var averageRentScore = _results["RentScores"];
                var averageSustainabilityScore = _results["SustainabilityScores"];
                _years = Enumerable.Range(0, horizon).Select(t => DateTime.Today.Year + t).Cast<object>().ToList();
                Console.WriteLine($"Years = {_years}");

                LoadScoresData(averageScore, averageNumberOfHousesScore, averageRentScore, averageSustainabilityScore);
                LoadAverageResultsData(horizon, numberOfScenarios);
                LoadBankruptcyData(horizon);
                LoadDebtData(horizon);

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

        private void LoadAverageResultsData(int horizon, int numberOfScenarios)
        {
            averageResultsData = new List<ITrace>();

            AddScatterTrace(averageResultsData, _results["SolvencyRatio"], "Solvency ratio");
            AddScatterTrace(averageResultsData, _results["Houses"], "Number of houses");
            AddScatterTrace(averageResultsData, _results["Rent"], "Rent");
            AddScatterTrace(averageResultsData, _results["Sustainability"], "Sustainability");
            AddScatterTrace(averageResultsData, _results["Maintenance"], "Maintenance");
            AddScatterTrace(averageResultsData, _results["NumberOfBadHouses"], "NumberOfBadHouses");
        }

        private void LoadBankruptcyData(int horizon)
        {
            bankruptData = new List<ITrace>();
            var data = _results["NumberOfBankruptcies"].Cast<object>().ToList();
            AddScatterBar(bankruptData, data, "Bankruptcies");
        }

        private void LoadDebtData(int horizon)
        {
            debtData = new List<ITrace>();
            AddScatterTrace(debtData, _results["Debt"], "Debt");
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
