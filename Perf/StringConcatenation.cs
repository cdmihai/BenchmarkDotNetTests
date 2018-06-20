using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Perf
{
    [ClrJob]
    public class StringConcatenation
    {
        public string _initial = "string";

        [Params(2, 4, 8)]
        public int ConcatenationTimes { get; set; }

        private string PlusConcatenate(string initial, int concatenationTimes)
        {
            var s = initial;
            for (var i = 0; i < concatenationTimes - 1; i++)
            {
                s += s;
            }

            return s;
        }

        private string StringBufferConcatenate(string initial, int concatenationTimes)
        {
            var sb = new StringBuilder(initial.Length * concatenationTimes);
            sb.Append(initial);

            for (var i = 0; i < concatenationTimes - 1; i++)
            {
                sb.Append(initial);
            }

            return sb.ToString();
        }

        private string PlusConcatenateTwo(string initial)
        {
            return initial + initial;
        }

        private string PlusConcatenateFour(string initial)
        {
            return initial + initial + initial + initial;
        }

        private string PlusConcatenateEight(string initial)
        {
            return initial + initial + initial + initial + initial + initial + initial + initial;
        }

        private string BufferConcatenateTwo(string initial)
        {
            var sb = new StringBuilder(initial.Length * 2);
            sb.Append(initial).Append(initial);

            return sb.ToString();
        }

        private string BufferConcatenateFour(string initial)
        {
            var sb = new StringBuilder(initial.Length * 4);
            sb.Append(initial).Append(initial).Append(initial).Append(initial);

            return sb.ToString();
        }

        private string BufferConcatenateEight(string initial)
        {
            var sb = new StringBuilder(initial.Length * 8);
            sb.Append(initial).Append(initial).Append(initial).Append(initial).Append(initial).Append(initial).Append(initial).Append(initial);

            return sb.ToString();
        }

        [Benchmark]
        public string Plus()
        {
            return PlusConcatenate(_initial, ConcatenationTimes);
        }

        [Benchmark]
        public string StringBuilder()
        {
            return StringBufferConcatenate(_initial, ConcatenationTimes);
        }

        [Benchmark]
        public string PlusTwo()
        {
            return PlusConcatenateTwo(_initial);
        }

        [Benchmark]
        public string PlusFour()
        {
            return PlusConcatenateFour(_initial);
        }

        [Benchmark]
        public string PlusEight()
        {
            return PlusConcatenateEight(_initial);
        }

        [Benchmark]
        public string BufferTwo()
        {
            return BufferConcatenateTwo(_initial);
        }

        [Benchmark]
        public string BufferFour()
        {
            return BufferConcatenateFour(_initial);
        }

        [Benchmark]
        public string BufferEight()
        {
            return BufferConcatenateEight(_initial);
        }
    }
}