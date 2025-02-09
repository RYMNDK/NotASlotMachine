
using boxs.Models;
using System.Text;

namespace boxs.Services
{
    public interface IGenerateSpinResult
    {
        public abstract int[,] GetSpinResult(boxMachine configuration);
        public abstract String GenerateCSV(int[,] results);
    }

    public class GenerateSpinResult : IGenerateSpinResult
    {
        public GenerateSpinResult()
        {

        }

        public int[,] GetSpinResult(boxMachine configuration)
        {
            int[,] result = new int[configuration.maxY, configuration.maxX];

            Parallel.For(0, configuration.maxY, i =>
            {
                for (int j = 0; j < configuration.maxX; j++)
                {
                    result[i, j] = Random.Shared.Next(0, 10);
                }
            });

            return result;
        }

        public String GenerateCSV(int[,] results)
        {
            int rows = results.GetLength(0);
            int cols = results.GetLength(1);

            var rowsCsv = Enumerable.Range(0, rows)
                .Select(i =>
                    string.Join(",", Enumerable.Range(0, cols)
                    .Select(j => results[i, j].ToString()))
                );
            return string.Join(Environment.NewLine, rowsCsv);
        }

    }
}
