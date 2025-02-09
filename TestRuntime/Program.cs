using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections.Concurrent;

public class Program {
    static void Main(String[] args)
    {

        // arrange
        int[,] myArray = GetSpinResult(1000, 1000);
        //new int[,]
        //{
        //    { 5, 1, 5, 1, 5 },
        //    { 3, 5, 3, 5, 3 },
        //    { 5, 3, 5, 5, 3 }
        //};


        Stopwatch sw = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();

        // act
        sw.Start();
        var totalWins = TotalWinsBF(myArray);
        sw.Stop();

        sw2.Start();
        var totalWins2 = TotalWinsMemo(myArray);
        sw2.Stop();

        // assert
        // Print2DArray(myArray);
        Console.WriteLine("TW: " + totalWins + ", RT: " + sw.ElapsedTicks + " ticks");
        Console.WriteLine("TW: " + totalWins2 + ", RT: " + sw2.ElapsedTicks + " ticks");
    }

    private static int[] WavePeriod(int y) {
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


    public static long TotalWinsMemo(int[,] result) {
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

// use this as test oracle
public static long TotalWinsBF(int[,] result) {
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
            for (int j = 1; j < x; j++){

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

    // print the result for manual debugging
    public static void Print2DArray(int[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write(array[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    // get spin results
    public static int[,] GetSpinResult(int x, int y)
    {
        int[,] result = new int[y, x];

        Parallel.For(0, y, i =>
        {
            for (int j = 0; j < x; j++)
            {
                result[i, j] = Random.Shared.Next(0, 10);
            }
        });

        return result;
    }

}

