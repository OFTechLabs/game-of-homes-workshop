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

        PlotlyChart _averageCapitalChart;
        Config _averageCapitalConfig = new Config()
        {
            Responsive = true
        };
        Layout _averageCapitalLayout = CreateLayout(
            @"The average maintenance costs of all owned houses and scenarios,
        more costs worsen the financial situation of the association.
        And it shows the average rent of all owned houses and scenarios.
        A higher rent improves the financial situation of the association.");
        IList<ITrace> _averageCapitalData;

        PlotlyChart _averageNumbersChart;
        Config _averageNumbersConfig = new Config()
        {
            Responsive = true
        };
        Layout _averageNumbersLayout = CreateLayout("The average number of (bad) houses of 100 scenarios.");
        IList<ITrace> _averageNumbersData;

        PlotlyChart _averageSustainabilityChart;
        Config _averageSustainabilityConfig = new Config()
        {
            Responsive = true
        };
        Layout _averageSustainabilityLayout = CreateLayout("The average sustainability of 100 scenarios.");
        IList<ITrace> _averageSustainabilityData;

        PlotlyChart _averageSolvencyChart;
        Config _averageSolvencyConfig = new Config()
        {
            Responsive = true
        };
        Layout _averageSolvencyLayout = CreateLayout("The average solvency ratio of 100 scenarios. The association is bankrupt as soon as the solvency drops below 20% (regardless of future developments)");
        IList<ITrace> _averageSolvencyData;

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

        private static Layout CreateLayout(string caption)
        {
            return new Layout
            {
                XAxis = new List<XAxis>
                {
                    new XAxis
                    {
                        Title = new Plotly.Blazor.LayoutLib.XAxisLib.Title
                        {
                                Text = caption
                        } 
                    } 
                }
            };
        }

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
            
                LoadAverageScoresData(averageScore, averageNumberOfHousesScore, averageRentScore, averageSustainabilityScore);
                LoadAverageCapitalData(horizon);
                LoadAverageNumbersData(horizon);
                LoadAverageSolvencyRatioData(horizon);
                LoadAverageSustainabilityRatioData(horizon);
                LoadBankruptcyData(horizon);
                LoadDebtData(horizon);

                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        private void LoadAverageScoresData(List<object> averageScore, List<object> averageNumberOfHousesScore, List<object> averageRentScore,
            List<object> averageSustainabilityScore)
        {
            _scoresData = new List<ITrace>();

            AddScatterTrace(_scoresData, averageScore, "Total score");
            AddScatterTrace(_scoresData, averageNumberOfHousesScore, "Number of houses score");
            AddScatterTrace(_scoresData, averageRentScore, "Rent score");
            AddScatterTrace(_scoresData, averageSustainabilityScore, "Sustainability score");
        }

        private void LoadAverageCapitalData(int horizon)
        {
            _averageCapitalData = new List<ITrace>();
            AddScatterTrace(_averageCapitalData, _results["Rent"], "Rent");
            AddScatterTrace(_averageCapitalData, _results["Maintenance"], "Maintenance costs");
        }

        private void LoadAverageNumbersData(int horizon)
        {
            _averageNumbersData = new List<ITrace>();
            AddScatterTrace(_averageNumbersData, _results["Houses"], "Number of houses");
            AddScatterTrace(_averageNumbersData, _results["NumberOfBadHouses"], "Number of bad houses");
        }

         private void LoadAverageSolvencyRatioData(int horizon)
         {
            _averageSolvencyData = new List<ITrace>();
            AddScatterTrace(_averageSolvencyData, _results["SolvencyRatio"], "Solvency ratio");
         }

          private void LoadAverageSustainabilityRatioData(int horizon)
          {
              _averageSustainabilityData = new List<ITrace>();
                AddScatterTrace(_averageSustainabilityData, _results["Sustainability"], "Sustainability");
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
