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
    public class MethodVersusMember
    {

        class Foo
        {
            public static Foo Instance = new Foo();

            public static Foo Singleton() => Instance;
        }

        [Benchmark]
        public object Method()
        {
            var hashCode = 0;
            for (int i = 0; i < 1000000 ; i++)
            {
                hashCode += Foo.Singleton().GetHashCode();
            }

            return hashCode;
        }

        [Benchmark]
        public object Member()
        {
            var hashCode = 0;
            for (int i = 0; i < 1000000 ; i++)
            {
                hashCode += Foo.Instance.GetHashCode();
            }

            return hashCode;
        }
    }
}