using System;

namespace GraphPlotter.Models.Functions
{
    public class SincFunction : IMathFunction
    {
        public string Title => "F(x)=sinc(x)";

        public double[] GetYs(double[] xs, ParametersModel parameters)
        {
            var result = new double[xs.Length];

            for (int i = 0; i < xs.Length; i++)
            {
                result[i] = parameters.Mult * Math.Sin(xs[i] * parameters.Oscillations + parameters.XOffset) / (xs[i] * parameters.Oscillations + parameters.XOffset) + parameters.YOffset;

                if ((xs[i] * parameters.Oscillations + parameters.XOffset) == 0)
                {
                    result[i] = parameters.Mult + parameters.YOffset;
                }
            }

            return result;
        }
    }
}
