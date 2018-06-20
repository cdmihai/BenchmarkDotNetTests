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
    public class GetFilesVsEnumerateFiles
    {
        private int[] _toArray;
        private static readonly string TempPath = Path.GetTempPath();

        [Benchmark]
        public object GetFilesNoPattern()
        {
            return Directory.GetFiles(TempPath);
        }

        [Benchmark]
        public object EnumerateFilesNoPattern()
        {
            return Directory.EnumerateFiles(TempPath).ToImmutableArray();
        }


        [Benchmark]
        public object GetDirectoriesNoPattern()
        {
            return Directory.GetDirectories(TempPath);
        }

        [Benchmark]
        public object EnumerateDirectoriesNoPattern()
        {
            return Directory.EnumerateDirectories(TempPath).ToImmutableArray();
        }

        [Benchmark]
        public object GetFilesPattern()
        {
            return Directory.GetFiles(TempPath, "*a*");
        }

        [Benchmark]
        public object EnumerateFilesPattern()
        {
            return Directory.EnumerateFiles(TempPath, "*a*").ToImmutableArray();
        }

        [Benchmark]
        public object GetDirectoriesPattern()
        {
            return Directory.GetDirectories(TempPath, "*a*");
        }

        [Benchmark]
        public object EnumerateDirectoriesPattern()
        {
            return Directory.EnumerateDirectories(TempPath, "*a*").ToImmutableArray();
        }
    }
}