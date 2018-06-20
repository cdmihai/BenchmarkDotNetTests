using System;
using BenchmarkDotNet.Attributes;

namespace Perf
{
    public class StructVsOut
    {
        private const int repetitions = 100000;

        private void ReturnAsOutParams(
            out string a,
            out string b,
            out string c,
            out string d,
            out bool e,
            out bool f)
        {
            a = "123456778";
            b = "123456778";
            c = "123456778";
            d = "123456778";

            e = false;
            f = true;
        }

        private struct ReturnStruct
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
            public bool E { get; set; }
            public bool F { get; set; }
        }

        private ReturnStruct ReturnAStruct()
        {
            var ret = new ReturnStruct
            {
                A = "123456778",
                B = "123456778",
                C = "123456778",
                D = "123456778",
                E = false,
                F = true
            };

            return ret;
        }

        private class ReturnClass
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
            public bool E { get; set; }
            public bool F { get; set; }
        }

        private ReturnClass ReturnAClass()
        {
            var ret = new ReturnClass
            {
                A = "123456778",
                B = "123456778",
                C = "123456778",
                D = "123456778",
                E = false,
                F = true
            };

            return ret;
        }

        [Benchmark]
        public bool ReturnAsOutParams()
        {
            bool ret = false;

            for (int i = 0; i < repetitions; i++)
            {
                bool e, f;
                string a, b, c, d;
                ReturnAsOutParams(out a, out b, out c, out d, out e, out f);

                ret |= e;
            }

            GC.Collect();

            return ret;
        }

        [Benchmark]
        public bool ReturnAsStruct()
        {
            bool ret = false;

            for (int i = 0; i < repetitions; i++)
            {
                var aStruct = ReturnAStruct();

                ret |= aStruct.E;
            }

            GC.Collect();

            return ret;
        }

        [Benchmark]
        public bool ReturnAsClass()
        {
            bool ret = false;

            for (int i = 0; i < repetitions; i++)
            {
                var aClass = ReturnAClass();

                ret |= aClass.E;
            }

            GC.Collect();

            return ret;
        }
    }
}