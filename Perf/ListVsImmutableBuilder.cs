using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.DotNet.InternalAbstractions;

namespace Perf
{
    [MemoryDiagnoser]
    public class ListVsImmutableBuilder
    {
        private int[] _toArray;
        private static readonly string TempPath = Path.GetTempPath();

        [Params(10, 100, 1000, 10000, 1000000)]
        public int ListSize { get; set; }

        public object[] Items;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Items = new object[ListSize];

            for (int i = 0; i < ListSize; i++)
            {
                Items[i] = new object();
            }
        }

        public void GlobalCleanup()
        {
            Items = null;
            GC.Collect();
        }

        [Benchmark]
        public object ListAdd()
        {
            var list = new List<object>();

            for (int i = 0; i < ListSize; i++)
            {
                list.Add(Items[i]);
            }

            return list;
        }

        [Benchmark]
        public object ImmutableListAdd()
        {
            var immutableListBuilder = ImmutableList.CreateBuilder<object>();

            for (int i = 0; i < ListSize; i++)
            {
                immutableListBuilder.Add(Items[i]);
            }

            return immutableListBuilder.ToImmutable();
        }

        [Benchmark]
        public object ImmutableArrayAdd()
        {
            var ImmutableArrayBuilder = ImmutableArray.CreateBuilder<object>();

            for (int i = 0; i < ListSize; i++)
            {
                ImmutableArrayBuilder.Add(Items[i]);
            }

            return ImmutableArrayBuilder.ToImmutable();
        }
    }
}