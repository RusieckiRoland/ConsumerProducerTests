using ConsumerProducerTestApp.MockBurden;
using ConsumerProducerTestApp.Utility;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ConsumerProducerTestApp.ClientSide
{
    public static class MainUtitilty
    {
        public static async Task<string> RunAsync(bool Wait = true)
        {
            var collection = Enumerable.Range(1, 300).ToList();
            var heavyResponse = new HeavyResponse();
            heavyResponse.Wait = Wait;

            var stopwatch = new Stopwatch();
            var builder = new StringBuilder();
            var conDictionary = new ConcurrentDictionary<int, int>();

            stopwatch.Start();
            var loadingTasks = new List<Task>();

            foreach (var item in collection)
            {
                loadingTasks.Add(heavyResponse.GetResponseAsync(item, conDictionary));
            }

            await Task.WhenAll(loadingTasks);

            stopwatch.Stop();
            var elapsedTime1 = stopwatch.Elapsed;

            var translator = new HeavyTranslator();
            translator.Wait = Wait;
            stopwatch.Reset();
            stopwatch.Start();

            foreach (var item in conDictionary)
            {
                var textValue = translator.Translate(item.Value);
                Console.WriteLine(textValue);
                builder.Append(textValue);
            }
            var elapsedTime2 = stopwatch.Elapsed;
            Console.WriteLine($"Process one: {elapsedTime1} Process two {elapsedTime2}");
            return builder.ToString();
        }

        public static string Run(bool Wait = true)
        {
            var collection = Enumerable.Range(1, 300).ToList();
            var heavyResponse = new HeavyResponse();
            heavyResponse.Wait = Wait;

            var stopwatch = new Stopwatch();
            var builder = new StringBuilder();
            var intList = new List<int>();

            stopwatch.Start();

            foreach (var item in collection)
            {
                intList.Add(heavyResponse.GetResponse(item));
            }

            stopwatch.Stop();
            var elapsedTime1 = stopwatch.Elapsed;

            var translator = new HeavyTranslator();
            translator.Wait = Wait;
            stopwatch.Reset();
            stopwatch.Start();

            foreach (var item in intList)
            {
                var textValue = translator.Translate(item);
                Console.WriteLine(textValue);
                builder.Append(textValue);
            }
            var elapsedTime2 = stopwatch.Elapsed;
            Console.WriteLine($"Process one: {elapsedTime1} Process two {elapsedTime2}");
            return builder.ToString();
        }

        public static string RunProducerConsumer(bool Wait = true)
        {
            var collection = Enumerable.Range(1, 300).ToList();
            var heavyResponse = new HeavyResponse();
            heavyResponse.Wait = Wait;

            var stopwatch = new Stopwatch();
            var builder = new StringBuilder();
            var conDictionary = new ConcurrentDictionary<int, string>();

            stopwatch.Start();
            TranslateQueue translateQueue = new TranslateQueue(conDictionary);
            var loadingTasks = new List<Task>();

            foreach (var item in collection)
            {
                loadingTasks.Add(heavyResponse.GetResponseForConsumer(item, translateQueue));
            }

            Task.WaitAll(loadingTasks.ToArray());
            stopwatch.Stop();
            var elapsedTime1 = stopwatch.Elapsed;
            translateQueue.SetNoMoreComputations();
            stopwatch.Reset();
            stopwatch.Start();

            Task[] translatingTasks =
        {
                Task.Run(() => translateQueue.Translate()),
                Task.Run(() => translateQueue.Translate()),
                Task.Run(() => translateQueue.Translate()),
                Task.Run(() => translateQueue.Translate())
            };
            Task.WaitAll(translatingTasks);

            conDictionary.ToList().ForEach(element =>
            {
                Console.WriteLine(element.Value);
                builder.Append(element.Value);
            });

            var elapsedTime2 = stopwatch.Elapsed;
            Console.WriteLine($"Process one: {elapsedTime1} Process two {elapsedTime2}");
            return builder.ToString();
        }
    }
}