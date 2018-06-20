using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Perf
{
    [MemoryDiagnoser]
    public class StringNormalizer
    {
        private static string s_directorySeparator = new string(Path.DirectorySeparatorChar, 1);
        internal static readonly char[] directorySeparatorCharacters = { '/', '\\' };

        internal static string Normalize(string aString)
        {
            if (string.IsNullOrEmpty(aString))
            {
                return aString;
            }

            var sb = new StringBuilder(aString.Length);
            var index = 0;

            // preserve meaningful roots and their slashes
            if (aString.Length >= 2 && IsValidDriveChar(aString[0]) && aString[1] == ':')
            {
                sb.Append(aString[0]);
                sb.Append(aString[1]);

                var i = SkipCharacters(aString, 2, c => IsSlash(c));

                if (index != i)
                {
                    sb.Append('\\');
                }

                index = i;
            }
            else if (aString.StartsWith("/", StringComparison.Ordinal))
            {
                sb.Append('/');
                index = SkipCharacters(aString, 1, c => IsSlash(c));
            }
            else if (aString.StartsWith(@"\", StringComparison.Ordinal))
            {
                sb.Append(@"\");
                index = SkipCharacters(aString, 1, c => IsSlash(c));
            }
            else if (aString.StartsWith(@"\\", StringComparison.Ordinal))
            {
                sb.Append(@"\\");
                index = SkipCharacters(aString, 2, c => IsSlash(c));
            }

            while (index < aString.Length)
            {
                var afterSlashesIndex = SkipCharacters(aString, index, c => IsSlash(c));

                // do not append separator at the end of the string
                if (afterSlashesIndex >= aString.Length)
                {
                    break;
                }
                // replace multiple slashes with the OS separator
                else if (afterSlashesIndex > index)
                {
                    sb.Append(s_directorySeparator);
                }

                var afterNonSlashIndex = SkipCharacters(aString, afterSlashesIndex, c => !IsSlash(c));

                sb.Append(aString, afterSlashesIndex, afterNonSlashIndex - afterSlashesIndex);

                index = afterNonSlashIndex;
            }

            return sb.ToString();
        }

        private static bool IsSlash(char c) => c == '/' || c == '\\';

        /// <summary>
        /// Skips characters that satisfy the condition <param name="jumpOverCharacter"></param>
        /// </summary>
        /// <param name="aString">The working string</param>
        /// <param name="startingIndex">Offset in string to start the search in</param>
        /// <returns>First index that does not satisfy the condition. Returns the string's length if end of string is reached</returns>
        private static int SkipCharacters(string aString, int startingIndex, Func<char, bool> jumpOverCharacter)
        {
            var index = startingIndex;

            while (index < aString.Length && jumpOverCharacter(aString[index]))
            {
                index++;
            }

            return index;
        }

        // copied from https://github.com/dotnet/corefx/blob/master/src/Common/src/System/IO/PathInternal.Windows.cs#L77-L83
        /// <summary>
        /// Returns true if the given character is a valid drive letter
        /// </summary>
        internal static bool IsValidDriveChar(char value)
        {
            return ((value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z'));
        }

        internal static string NormalizeViaSplits(string aString)
        {
            if (string.IsNullOrEmpty(aString))
            {
                return aString;
            }

            var header = string.Empty;

            // preserver meaningful root slashes
            if (aString.StartsWith(@"\\"))
            {
                header = @"\\";
            }
            else if (aString.StartsWith("/"))
            {
                header = "/";
            }
            else if (aString.StartsWith(@"\"))
            {
                header = @"\";
            }

            var splits = aString.Split(directorySeparatorCharacters, StringSplitOptions.RemoveEmptyEntries);

            if (splits.Length == 0)
            {
                splits = new string[0];
            }

            // do not loose the slash when the only split element is a drive letter
            if (splits.Length == 1 && splits[0].Length == 2 && splits[0][1] == ':')
            {
                splits[0] = splits[0] + "\\";
            }

            return header + splits.DefaultIfEmpty().Aggregate((a, b) => $"{a}{s_directorySeparator}{b}");
        }

        [Benchmark]
        public string ViaBuilder()
        {
            return Normalize(@"c:\/\a//b\/");
        }

        [Benchmark]
        public string ViaSplits()
        {
            return NormalizeViaSplits(@"c:\/\a//b\/");
        }
    }
}