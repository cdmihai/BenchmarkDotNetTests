using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace Perf
{
    class Program
    {
        static void Main(string[] args)
        {
            var results = BenchmarkRunner.Run<NativeVsManagedExistenceChecks>();
        }
    }
}