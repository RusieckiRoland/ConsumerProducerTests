using ConsumerProducerTestApp.Utility;

namespace ConsumerProducerTestApp.MockBurden
{
    public class HeavyTranslator
    {
        public bool Wait { get; set; } = true;

        public string Translate(int number)
        {
            var rnd = new Random();
            var timeSpan = rnd.Next(50);
            var stop = DateTime.Now.AddMilliseconds(timeSpan);
            var result = string.Empty;
            var once = true;
            while ((DateTime.Now <= stop) || once)
            {
                ConvertNumberToTextUtility.ConvertNumberToText(number, out var textRepresentation);
                result = textRepresentation;
                once = false;
                if (!Wait) break;
            }
            return result;
        }
    }
}