using ConsumerProducerTestApp.Utility;
using System.Collections.Concurrent;

namespace ConsumerProducerTestApp.MockBurden
{
    public class HeavyResponse
    {
        public bool Wait { get; set; } = true;

        public int GetResponse(int argument)
        {
            var rnd = new Random();
            var timeSpan = rnd.Next(200);
            var stop = DateTime.Now.AddMilliseconds(timeSpan);
            int result = 0;
            var once = true;
            while ((DateTime.Now <= stop) || once)
            {
                var sin = Math.Abs(Math.Sin(argument));
                result = (int)RoundUp(sin * argument, 0);
                once = false;
                if (!Wait) break;
            }
            return result + argument;
        }

        public Task<int> GetResponseAsync(int argument)
        {
            var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var result = GetResponse(argument);
                tcs.SetResult(result);
            });
            return tcs.Task;
        }

        public Task GetResponseAsync(int argument, ConcurrentDictionary<int, int> conDictionary)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var result = GetResponse(argument);
                conDictionary.TryAdd(argument, result);
                tcs.SetResult();
            });
            return tcs.Task;
        }

        public Task GetResponseForConsumer(int argument, TranslateQueue translateQueue)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var result = GetResponse(argument);
                translateQueue.QueForTranslating(argument, result);
                tcs.SetResult();
            });
            return tcs.Task;
        }

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}