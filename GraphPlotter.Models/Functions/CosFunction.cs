using System;

namespace GraphPlotter.Models.Functions
{
    public class CosFunction : IMathFunction
    {
        public string Title => "F(x)=cos(x)";

        public double[] GetYs(double[] xs, ParametersModel parameters)
        {
            var result = new double[xs.Length];

            for (int i = 0; i < xs.Length; i++)
            {
                result[i] = parameters.Mult * Math.Cos(xs[i] * parameters.Oscillations + parameters.XOffset) + parameters.YOffset;
            }

            return result;
        }
    }
}
