using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace Perf
{
    [MemoryDiagnoser]
    [ClrJob]
    public class StringReplace
    {
        private static string aString = @"$(MsBuildExtensionsPath)\Plugin\Props\*";
        private static string oldValueCaseMissmatch = "$(MSBuildExtensionsPath)";
        private static string oldValueMissmatch = "$(MSBuildFooBarPath)";
        private static string oldValueMatch = "$(MsBuildExtensionsPath)";
        private static string newValue = @"c:\Program Files(x86)\MSBuild";

        public static string Replace(string aString, string oldValue, string newValue, StringComparison stringComparison)
        {
            if (newValue == null)
            {
                newValue = string.Empty;
            }

            var currentOccurrence = aString.IndexOf(oldValue, stringComparison);

            if (currentOccurrence == -1)
            {
                return aString;
            }

            var endOfPreviousOccurrence = 0;

            var builder = new StringBuilder(aString.Length);

            while (currentOccurrence != -1)
            {
                var nonMatchLength = currentOccurrence - endOfPreviousOccurrence;
                builder.Append(aString, endOfPreviousOccurrence, nonMatchLength);
                builder.Append(newValue);

                endOfPreviousOccurrence = currentOccurrence + oldValue.Length;
                currentOccurrence = aString.IndexOf(oldValue, endOfPreviousOccurrence, stringComparison);
            }

            builder.Append(aString, endOfPreviousOccurrence, aString.Length - endOfPreviousOccurrence);

            return builder.ToString();
        }

        [Setup]
        public void Setup()
        {
        }

        [Benchmark]
        public object OldNonMatching()
        {
            return aString.Replace(oldValueCaseMissmatch, newValue);
        }

        [Benchmark]
        public object OldMatching()
        {
            return aString.Replace(oldValueMatch, newValue);
        }

        [Benchmark]
        public object NewCaseMissmatch()
        {
            return Replace(aString, oldValueCaseMissmatch, newValue, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public object NewNonMatching()
        {
            return Replace(aString, oldValueMissmatch, newValue, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public object NewMatching()
        {
            return Replace(aString, oldValueMatch, newValue, StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public object NewNonMatching_caseSensitive()
        {
            return Replace(aString, oldValueMissmatch, newValue, StringComparison.Ordinal);
        }

        [Benchmark]
        public object NewMatching_caseSensitive()
        {
            return Replace(aString, oldValueMatch, newValue, StringComparison.Ordinal);
        }
    }
}