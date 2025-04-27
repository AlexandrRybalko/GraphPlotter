namespace GraphPlotter.Models
{
    public interface IMathFunction
    {
        string Title { get; }

        double[] GetYs(double[] xs, ParametersModel parameters);
    }
}
