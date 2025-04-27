using System;

namespace GraphPlotter.Models.Functions
{
    public class SinFunction : IMathFunction
    {
        public string Title => "F(x)=sin(x)";

        public double[] GetYs(double[] xs, ParametersModel parameters)
        {
            var result = new double[xs.Length];

            for (int i = 0; i < xs.Length; i++)
            {
                result[i] = parameters.Mult * Math.Sin(xs[i] * parameters.Oscillations + parameters.XOffset) + parameters.YOffset;
            }

            return result;
        }
    }
}
