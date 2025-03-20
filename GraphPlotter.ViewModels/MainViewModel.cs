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

namespace GraphPlotter.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly double delta = 0.1;
        private readonly string parametersFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\prevPar.txt";
        private ParametersModel parameters;

        public IList<string> Functions { get; }
        public WpfPlot Plot { get; } = new WpfPlot();

        public int SelectedFunction
        {
            get => parameters.SelectedFunction;
            set
            {
                parameters.SelectedFunction = value;
                OnPropertyChanged(nameof(SelectedFunction));
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
        public RelayCommand SavePlotCommand { get; set; }

        public MainViewModel()
        {
            LoadPreviousParameters();
            Functions = new ObservableCollection<string>() { "F(x)=sin(x)", "F(x)=cos(x)", "F(x)=sinc(x)" };
            UpdatePlotCommand = new RelayCommand(UpdatePlotExecute);
            SavePlotCommand = new RelayCommand(SavePlotExecute, SavePlotCanExecute);
        }

        public void SaveState()
        {
            var p = JsonSerializer.Serialize<ParametersModel>(parameters);
            File.WriteAllText(parametersFilePath, p);
        }

        private void LoadPreviousParameters()
        {
            if (File.Exists(parametersFilePath))
            {
                var parametersString = File.ReadAllText(parametersFilePath);

                try
                {
                    var previousParameters = JsonSerializer.Deserialize<ParametersModel>(parametersString);
                    parameters = previousParameters;
                }
                catch (JsonException e)
                {
                    parameters = new ParametersModel() { Mult = 1, Oscillations = 1 };
                }
            }
            else
            {
                parameters = new ParametersModel() { Mult = 1, Oscillations = 1 };
            }
        }

        private void UpdatePlotExecute()
        {
            var xs = GetXs();
            var ys = new double[xs.Length];

            if (SelectedFunction == 0)
            {
                ys = GetSinYs(xs);
            }
            else if (SelectedFunction == 1)
            {
                ys = GetCosYs(xs);
            }
            else if (SelectedFunction == 2)
            {
                ys = GetSincYs(xs);
            }

            Plot.Plot.Clear();
            Plot.Plot.Add.SignalXY(xs, ys);
            Plot.Refresh();
            SavePlotCommand.NotifyCanExecuteChanged();
        }

        private bool SavePlotCanExecute() => Plot.Plot.GetPlottables().Any();

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
            var xsCount = (int)Math.Ceiling((Plot.Plot.Grid.XAxis.Max - Plot.Plot.Grid.XAxis.Min) / delta);
            var result = Generate.Consecutive(xsCount, delta, Plot.Plot.Grid.XAxis.Min);

            return result;
        }

        private double[] GetSinYs(double[] xs)
        {
            var result = new double[xs.Length];

            for (int i = 0; i < xs.Length; i++)
            {
                result[i] = Mult * Math.Sin(xs[i] * Oscillations + XOffset) + YOffset;
            }

            return result;
        }

        private double[] GetCosYs(double[] xs)
        {
            var result = new double[xs.Length];

            for (int i = 0; i < xs.Length; i++)
            {
                result[i] = Mult * Math.Cos(xs[i] * Oscillations + XOffset) + YOffset;
            }

            return result;
        }

        private double[] GetSincYs(double[] xs)
        {
            var result = new double[xs.Length];

            for (int i = 0; i < xs.Length; i++)
            {
                if (xs[i] == 0)
                {
                    result[i] = 1 * Mult + YOffset;
                }
                else
                {
                    //result[i] = Math.Sin(xs[i]) / xs[i] + YOffset;
                    result[i] = Mult * Math.Sin(xs[i] * Math.PI) / (xs[i] * Math.PI) + YOffset;
                }
            }

            return result;
        }
    }
}
