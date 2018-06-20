using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Perf
{
    public class Yield
    {
        private int[] _toArray;

        [Setup]
        public void Setup()
        {
            _toArray = Enumerable.Range(0, 10000).ToArray();
        }

        public static IEnumerable<int> ViaYieldOne()
        {
            yield return 1;
        }

        public static IEnumerable<int> ViaYieldTwo()
        {
            yield return 1;
            yield return 2;
        }

        public static IEnumerable<int> ViaPrecomputedArray()
        {
            return new int[]{1, 2};
        }

        [Benchmark]
        public object YieldOneTest()
        {
            var s = 0;

            foreach (var i in ViaYieldOne())
            {
                s += i;
            }

            return s;
        }

        [Benchmark]
        public object YieldTwoTest()
        {
            var s = 0;

            foreach (var i in ViaYieldTwo())
            {
                s += i;
            }

            return s;
        }

        [Benchmark]
        public object PrecomputedArrayTest()
        {
            var s = 0;

            foreach (var i in ViaPrecomputedArray())
            {
                s += i;
            }

            return s;
        }
    }
}