using ConsumerProducerTestApp.MockBurden;
using System.Collections.Concurrent;

namespace ConsumerProducerTestApp.Utility
{
    public class TranslateQueue
    {
        private HeavyTranslator _heavyTranslator = new HeavyTranslator();
        private BlockingCollection<(int key, int value)> _translateQueue = new BlockingCollection<(int key, int value)>();
        private ConcurrentDictionary<int, string> _resultDictionary;

        public void SetNoMoreComputations() => _translateQueue.CompleteAdding();

        public TranslateQueue(ConcurrentDictionary<int, string> concurentDict)
        {
            _resultDictionary = concurentDict;
        }

        public void QueForTranslating(int key, int value) => _translateQueue.TryAdd((key, value));

        public void Translate()
        {
            foreach (var item in _translateQueue.GetConsumingEnumerable())
            {
                var translatedValue = _heavyTranslator.Translate(item.value);
                _resultDictionary.TryAdd(item.key, translatedValue);
            }

            //regular implementation
            //while (true)
            //{
            //    try
            //    {
            //        var translateUnit = _translateQueue.Take();
            //        var translatedValue = _heavyTranslator.Translate(translateUnit.value);
            //        _resultDictionary.TryAdd(translateUnit.key, translatedValue);
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //}
        }
    }
}