using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Perf
{
    public class CombinedRegexes
    {
        private static readonly string EmailRegex = @"^(?<user>{0})@(?<server>[^\s\.]+)\.(?<domain>[^\s.]+)$";

        private static readonly Random Random = new Random(472882027);

        private static readonly int[] Counts = {10, 100, 1000, 10000};

        private readonly Dictionary<int, State> _state;

        [Params(10, 100, 1000, 10000)]
        public int CurrentCount { get; set; }

        public CombinedRegexes()
        {
            _state = Counts.ToDictionary(
                i => i,
                count =>
                {
                    var names = Enumerable.Range(0, count).Select(i => $"user_name_{i}").ToArray();

                    var regexStrings = names.Select(n => string.Format(EmailRegex, n)).ToArray();
                    var regexObjects = regexStrings.Select(_ => new Regex(_, RegexOptions.Compiled));
                    var combinedRegexString = regexStrings.Aggregate((r1, r2) => $"({r1})|({r2})");
                    var combinedRegex = new Regex(combinedRegexString, RegexOptions.Compiled);

                    var matchingStrings = names.Select(n => $"{n}@foobar.com");
                    var nonMatchingStrings = names.Select(n => $"{n}@foobar.com.");

                    return new State
                    {
                        MatchingStrings = matchingStrings.ToArray(),
                        NonMatchingStrings = nonMatchingStrings.ToArray(),
                        Regexes = regexObjects.ToArray(),
                        CombinedRegex = combinedRegex
                    };
                });
        }

        [Benchmark]
        public bool MultipleRegexes_Matching()
        {
            var currentState = _state[CurrentCount];
            return MultipleRegexes(currentState.MatchingStrings, currentState.Regexes, true);
        }

        [Benchmark]
        public bool MultipleRegexes_NonMatching( )
        {
            var currentState = _state[CurrentCount];
            return MultipleRegexes(currentState.NonMatchingStrings, currentState.Regexes, false);
        }

        private bool MultipleRegexes(IEnumerable<string> strings, IEnumerable<Regex> regexes, bool allShouldMatch)
        {
            var allMatch = strings.All(s => regexes.AsParallel().Any(r => r.IsMatch(s)));

            if (allShouldMatch)
            {
                Debug.Assert(allMatch);
            }
            else
            {
                Debug.Assert(!allMatch);
            }

            return allMatch;
        }

        [Benchmark]
        public bool CombinedRegex_Matching()
        {
            var currentState = _state[CurrentCount];
            return CombinedRegex(currentState.MatchingStrings, currentState.CombinedRegex, true);
        }

        [Benchmark]
        public bool CombinedRegex_NonMatching()
        {
            var currentState = _state[CurrentCount];
            return CombinedRegex(currentState.NonMatchingStrings, currentState.CombinedRegex, false);
        }

        private bool CombinedRegex(IEnumerable<string> strings, Regex regex, bool allShouldMatch)
        {
            var allMatch = strings.All(regex.IsMatch);

            if (allShouldMatch)
            {
                Debug.Assert(allMatch);
            }
            else
            {
                Debug.Assert(!allMatch);
            }

            return allMatch;
        }

        private struct State
        {
            public IEnumerable<string> MatchingStrings { get; set; }
            public IEnumerable<string> NonMatchingStrings { get; set; }
            public IEnumerable<Regex> Regexes { get; set; }
            public Regex CombinedRegex { get; set; }
        }
    }
}