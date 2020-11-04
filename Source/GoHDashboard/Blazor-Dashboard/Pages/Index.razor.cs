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

        PlotlyChart _scoresChart;
        Config _scoresConfig = new Config()
        {
            Responsive = true
        };
        Layout _scoresLayout = new Layout();
        // Using of the interface IList is important for the event callback!
        IList<ITrace> _scoresData;

        PlotlyChart _averageResultsChart;
        Config _averageResultsConfig = new Config()
        {
            Responsive = true
        };
        Layout _averageResultsLayout = new Layout();
        IList<ITrace> _averageResultsData;

        PlotlyChart _bankruptPerYearChart;
        Config _bankruptPerYearConfig = new Config()
        {
            Responsive = true
        };
        Layout _bankruptPerYearLayout = new Layout()
        {
            BarMode = BarModeEnum.Stack
        };

        bool _containsBankruptScenarios;

        IList<ITrace> _bankruptPerYearData;

        PlotlyChart _bankruptTotalChart;
         Config _bankruptTotalConfig = new Config()
        {
            Responsive = true
        };
        Layout _bankruptTotalLayout = new Layout()
        {
            BarMode = BarModeEnum.Stack
        };
         IList<ITrace> _bankruptTotalData;

        PlotlyChart _debtChart;

        Config _debtConfig = new Config()
        {
            Responsive = true
        };

        Layout _debtLayout = new Layout();
    
        IList<ITrace> _debtData;

        [Inject]
        private HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var jsonString = await HttpClient.GetStringAsync("results.json");
                _results = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonString).ToDictionary(p => p.Key, p => p.Value.Cast<object>().ToList());
                var horizon = _results.Values.First().Count;
                
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
            _scoresData = new List<ITrace>();

            AddScatterTrace(_scoresData, averageScore, "Average total score");
            AddScatterTrace(_scoresData, averageNumberOfHousesScore, "Average number of houses score");
            AddScatterTrace(_scoresData, averageRentScore, "Average rent score");
            AddScatterTrace(_scoresData, averageSustainabilityScore, "Average sustainability score");
        }

        private void LoadAverageResultsData(int horizon, int numberOfScenarios)
        {
            _averageResultsData = new List<ITrace>();

            AddScatterTrace(_averageResultsData, _results["SolvencyRatio"], "Solvency ratio");
            AddScatterTrace(_averageResultsData, _results["Houses"], "Number of houses");
            AddScatterTrace(_averageResultsData, _results["Rent"], "Rent");
            AddScatterTrace(_averageResultsData, _results["Sustainability"], "Sustainability");
            AddScatterTrace(_averageResultsData, _results["Maintenance"], "Maintenance costs");
            AddScatterTrace(_averageResultsData, _results["NumberOfBadHouses"], "Number of bad houses");
        }

        private void LoadBankruptcyData(int horizon)
        {
            _bankruptPerYearData = new List<ITrace>();
            var data = _results["NumberOfBankruptcies"];
            AddScatterBar(_bankruptPerYearData, data, "Bankruptcies");

            var numberOfBankruptcies = _results["NumberOfBankruptcies"].Cast<double>().Last();
            _containsBankruptScenarios = numberOfBankruptcies > 0;
            _bankruptTotalData = new List<ITrace>
            {
                new Pie
                {
                    Labels = new List<object>{ "Bankrupt", "Survived"},
                    Values = new List<object>{ numberOfBankruptcies, 100 - numberOfBankruptcies}
                }
            };
        }

        private void LoadDebtData(int horizon)
        {
            _debtData = new List<ITrace>();
            AddScatterTrace(_debtData, _results["Debt"], "Debt");
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
