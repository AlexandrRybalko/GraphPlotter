using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ScottPlot;
using ScottPlot.WPF;
using GraphPlotter.Models;
using System.Windows;

namespace GraphPlotter.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly int numberOfPoints = 401;
        private readonly string parametersFileName = "prevPar.json";
        private readonly string appName = "GraphPlotter";
        private ParametersModel parameters;
        private List<IMathFunction> functions;

        public ObservableCollection<string> FunctionTitleList { get; }
        public WpfPlot Plot { get; } = new WpfPlot();

        public int SelectedFunctionIndex
        {
            get => parameters.SelectedFunctionIndex;
            set
            {
                parameters.SelectedFunctionIndex = value;
                OnPropertyChanged(nameof(SelectedFunctionIndex));
            }
        }

        public double Oscillations
        {
            get => parameters.Oscillations;
            set
            {
                parameters.Oscillations = value;
                OnPropertyChanged(nameof(Oscillations));
            }
        }

        public double Mult
        {
            get => parameters.Mult;
            set
            {
                parameters.Mult = value;
                OnPropertyChanged(nameof(Mult));
            }
        }

        public double YOffset
        {
            get => parameters.YOffset;
            set
            {
                parameters.YOffset = value;
                OnPropertyChanged(nameof(YOffset));
            }
        }

        public double XOffset
        {
            get => parameters.XOffset;
            set
            {
                parameters.XOffset = value;
                OnPropertyChanged(nameof(XOffset));
            }
        }

        public RelayCommand UpdatePlotCommand { get; set; }
        public RelayCommand UpdatePlotOnInteractionCommand { get; set; }
        public RelayCommand SavePlotCommand { get; set; }

        public MainViewModel()
        {
            functions = new List<IMathFunction>() { new Models.Functions.SinFunction(), new Models.Functions.CosFunction(), new Models.Functions.SincFunction() };
            FunctionTitleList = new ObservableCollection<string>(functions.Select(x => x.Title).ToList());
            LoadPreviousParameters();

            UpdatePlotCommand = new RelayCommand(UpdatePlotExecute);
            UpdatePlotOnInteractionCommand = new RelayCommand(UpdatePlotExecute, () => Plot.Plot.GetPlottables().Any());
            SavePlotCommand = new RelayCommand(SavePlotExecute, () => Plot.Plot.GetPlottables().Any());
        }

        public void SaveState()
        {
            var localDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolderPath = Path.Combine(localDataPath, appName);

            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }

            var localApplicationDataFilePath = Path.Combine(appFolderPath, parametersFileName);
            var serializedParameters = JsonSerializer.Serialize<ParametersModel>(parameters);
            File.WriteAllText(localApplicationDataFilePath, serializedParameters);
        }

        private void LoadPreviousParameters()
        {
            var localDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localApplicationDataFilePath = Path.Combine(localDataPath, appName, parametersFileName);

            parameters = new ParametersModel() { Mult = 1, Oscillations = 1 };

            if (File.Exists(localApplicationDataFilePath))
            {
                var parametersString = File.ReadAllText(localApplicationDataFilePath);

                try
                {
                    var previousParameters = JsonSerializer.Deserialize<ParametersModel>(parametersString);

                    if (previousParameters.SelectedFunctionIndex < 0 || previousParameters.SelectedFunctionIndex >= functions.Count)
                    {
                        previousParameters.SelectedFunctionIndex = 0;
                    }

                    parameters = previousParameters;
                }
                catch (JsonException e)
                {
                    MessageBox.Show("Unable to read previous parameters");
                }
            }
        }

        private void UpdatePlotExecute()
        {
            var xs = GetXs();
            var ys = functions[SelectedFunctionIndex].GetYs(xs, parameters);

            Plot.Plot.Clear();
            Plot.Plot.Add.SignalXY(xs, ys);
            Plot.Refresh();
            SavePlotCommand.NotifyCanExecuteChanged();
        }

        private void SavePlotExecute()
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = ".svg";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                Plot.Plot.SaveSvg(dialog.FileName, 800, 800);
            }
        }

        private double[] GetXs()
        {
            var delta = (Plot.Plot.Grid.XAxis.Max - Plot.Plot.Grid.XAxis.Min) / (numberOfPoints - 1);
            var result = Generate.Consecutive(numberOfPoints, delta, Plot.Plot.Grid.XAxis.Min);

            return result;
        }
    }
}