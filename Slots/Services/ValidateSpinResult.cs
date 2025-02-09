using System.Collections.Concurrent;

namespace boxs.Services
{
    public interface IValidateSpinResult
    {
        public abstract long GetTotalWins(int[,] result);
    }

    public class ValidateSpinResult : IValidateSpinResult
    {
        public ValidateSpinResult()
        {

        }

        private static int[] WavePeriod(int y)
        {
            List<int> ints = new List<int>();
            bool goDown = true;
            ints.Add(0);
            int counter = 1;
            while (counter > 0)
            {
                ints.Add(counter);
                if (goDown)
                {
                    counter++;

                    if (counter + 1 == y)
                    {
                        goDown = false;
                    }
                }
                else
                {
                    counter--;
                }
            }

            // return a fixed size array
            return ints.ToArray();
        }

        // parallel sum with a memo
        public long GetTotalWins(int[,] result)
        {
            int y = result.GetLength(0);
            int x = result.GetLength(1);
            int[] wavePeriod = WavePeriod(y);

            ConcurrentDictionary<int, int> keyValuePairs = new ConcurrentDictionary<int, int>();
            // key will be y * N -> x, value will shrink as we traverse array, N = 1 wave win, N = 2 line win 
            for (int i = 0; i < y * 2; i++)
            {
                keyValuePairs.GetOrAdd(i, key => x);
            }

            // the idea is after processing wave win we can know max possible value for line win
            Parallel.For(0, y, locy => {
                int first = result[locy, 0];

                for (int locx = 1; locx < x; locx++)
                {
                    int curY = wavePeriod[(locx + locy) % wavePeriod.Length];
                    int value = result[curY, locx];

                    // reduce linear search space
                    if (value != result[curY, 0])
                    {
                        keyValuePairs.AddOrUpdate(
                             y + curY,
                             addValue: x,
                             updateValueFactory: (k, prevValue) => Math.Min(locx, prevValue)
                        );
                    }

                    // break the sequence
                    if (value != first)
                    {
                        keyValuePairs.AddOrUpdate(
                             locy,
                             addValue: x,
                             updateValueFactory: (k, prevValue) => Math.Min(locx, prevValue)
                        );
                        break;
                    }
                }
            });

            Parallel.ForEach(keyValuePairs.ToArray(), kvp =>
            {
                // horizontal sum, the key holds max possible win at this point
                if (kvp.Key >= y)
                {
                    int count = 1;
                    for (int i = 1; i < kvp.Value; i++)
                    {
                        if (result[kvp.Key - y, i] == result[kvp.Key - y, 0])
                        {
                            count++;
                        }
                        else
                        {
                            keyValuePairs.AddOrUpdate(
                                 kvp.Key,
                                 addValue: x,
                                 updateValueFactory: (k, prevValue) => Math.Min(count, prevValue)
                            );
                            break;
                        }
                    }
                }
            });

            // sum up the dictionary
            return keyValuePairs.AsParallel().Sum(key => key.Value == 1 ? 0 : key.Value * result[key.Key % y, 0]);
        }

        // brute force serial approach
        public long GetTotalWinsBF(int[,] result)
        {
            long AllCount = 0;
            int y = result.GetLength(0);
            int x = result.GetLength(1);
            int[] wavePeriod = WavePeriod(y);

            // line
            for (int i = 0; i < y; i++)
            {
                int first = result[i, 0];
                int count = 1;
                for (int j = 1; j < x; j++)
                {
                    if (first == result[i, j])
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Only add if there is more than one occurrence
                if (count > 1)
                {
                    AllCount += first * count;
                }
            }

            // wave
            for (int i = 0; i < y; i++)
            {
                int count = 1;
                int first = result[i, 0];
                for (int j = 1; j < x; j++)
                {

                    int curY = wavePeriod[(j + i) % wavePeriod.Length];
                    if (first == result[curY, j])
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Only add if there is more than one occurrence
                if (count > 1)
                {
                    AllCount += first * count;
                }
            }

            return AllCount;
        }
    }
}
