using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Perf
{
    [MemoryDiagnoser]
    public class ImmutableListBuilderRemoveAll
    {
        private ImmutableList<Tuple<string>> itemsTemplate;
        private ImmutableList<string> itemsToRemoveTemplate;

        [Params(100, 1000)]
        public int ItemNumber {get; set;}

        [GlobalSetup]
        public void Setup()
        {
            var itemsNoX = Enumerable.Range(0, ItemNumber / 2).Select(i => $"{i}");
            var itemsX = Enumerable.Range(0, ItemNumber / 2).Select(i => $"x{i}");

            itemsTemplate = itemsNoX.Concat(itemsX).Select(i => Tuple.Create(i)).ToImmutableList();

            itemsToRemoveTemplate = itemsX.ToImmutableList();
        }

        [Benchmark]
        public object RemoveAll()
        {
            var items = ImmutableList.CreateBuilder<Tuple<string>>();
            items.AddRange(this.itemsTemplate);

            var itemsToRemove = ImmutableList.CreateRange(this.itemsToRemoveTemplate);

            return items.RemoveAll(p => itemsToRemove.Contains(p.Item1));
        }

        [Benchmark]
        public object Other()
        {
            var items = ImmutableList.CreateBuilder<Tuple<string>>();
            items.AddRange(this.itemsTemplate);

            var itemsToRemove = ImmutableList.CreateRange(this.itemsToRemoveTemplate);

            var itemDataToRemove = new List<Tuple<string>>();	
            foreach (var itemData in items)	
            {	
                if (itemsToRemove.Contains(itemData.Item1))	
                {	
                    itemDataToRemove.Add(itemData);	
                }	
            }	

            foreach (var itemToRemove in itemDataToRemove)	
            {	
                items.Remove(itemToRemove);	
            }

            return itemDataToRemove;
        }
    }
}