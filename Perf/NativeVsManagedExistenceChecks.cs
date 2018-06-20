using System;
using System.Collections.Generic;
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

    public class MultipleRuntimes : ManualConfig
    {
        public MultipleRuntimes()
        {
            Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp21));

            // Add(Job.Default.With(CsProjClassicNetToolchain.Net462));
        }
    }

    [Config(typeof(MultipleRuntimes))]
    [MemoryDiagnoser]
    public class NativeVsManagedExistenceChecks
    {
        private string[] _existingDirectories;
        private string[] _existingFiles;
        private string[] _nonExistingDirectories;
        private string[] _nonExistringFiles;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _existingDirectories = File.ReadAllLines(@"e:\projects\directories");
            _existingFiles = File.ReadAllLines(@"e:\projects\files");
            _nonExistingDirectories = Enumerable.Range(1, 1000).Select(i => @"e:\projects\dir{i}").ToArray();
            _nonExistringFiles = Enumerable.Range(1, 1000).Select(i => @"e:\projects\dir{i}").ToArray();
        }

        private bool Exists(Func<string, bool> existenceCheck, string[] entries)
        {
            var allExist = false;

            foreach (var entry in entries)
            {
                allExist |= existenceCheck(entry);
            }

            return allExist;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WIN32_FILE_ATTRIBUTE_DATA
        {
            internal int fileAttributes;
            internal uint ftCreationTimeLow;
            internal uint ftCreationTimeHigh;
            internal uint ftLastAccessTimeLow;
            internal uint ftLastAccessTimeHigh;
            internal uint ftLastWriteTimeLow;
            internal uint ftLastWriteTimeHigh;
            internal uint fileSizeHigh;
            internal uint fileSizeLow;
        }

        internal const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetFileAttributesEx(String name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

        private bool DirectoryExistsNative(string entry)
        {
            WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA();
            var success = GetFileAttributesEx(entry, 0, ref data);

            if (success)
            {
                return ((data.fileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0);
            }

            return false;
        }

        private bool FileExistsNative(string entry)
        {
            WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA();
            var success = GetFileAttributesEx(entry, 0, ref data);

            if (success)
            {
                return ((data.fileAttributes & FILE_ATTRIBUTE_DIRECTORY) == 0);
            }

            return false;
        }

        private bool FileOrDirectoryExistsNative (string path)
        {
            WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA();
            return GetFileAttributesEx(path, 0, ref data);
        }

        private bool DirectoryExistsAttributes(string path)
        {
            return GetFileAttributes(path) == GetFileAttributesResult.Directory;
        }

        private bool FileExistsAttributes(string path)
        {
            return GetFileAttributes(path) == GetFileAttributesResult.File;
        }

        private bool FileOrDirectoryExistsAttributes (string path)
        {
            return GetFileAttributes(path) != GetFileAttributesResult.Error;
        }

        private static GetFileAttributesResult GetFileAttributes(string fullPath)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(fullPath);
                bool isDirectory = (attributes & FileAttributes.Directory) != 0;
                return isDirectory ? GetFileAttributesResult.Directory : GetFileAttributesResult.File;
            }
            catch
            {
                return GetFileAttributesResult.Error;
            }
        }

        private enum GetFileAttributesResult
        {
            Directory,
            Error,
            File,
        }

        [Benchmark]
        public object ManagedDirectoryExists()
        {
            return Exists(Directory.Exists, _existingDirectories);
        }

        [Benchmark]
        public object ManagedFileExists()
        {
            return Exists(File.Exists, _existingFiles);
        }

        [Benchmark]
        public object ManagedFileOrDirectoryExist()
        {
            return Exists(p => File.Exists(p) || Directory.Exists(p), _existingFiles);
        }

        [Benchmark]
        public object ManagedDirectoryDoesNotExist()
        {
            return Exists(Directory.Exists, _nonExistingDirectories);
        }

        [Benchmark]
        public object ManagedFileDoesNotExist()
        {
            return Exists(File.Exists, _nonExistringFiles);
        }

        [Benchmark]
        public object ManagedFileOrDirectoryDoesNotExist()
        {
            return Exists(p => File.Exists(p) || Directory.Exists(p), _nonExistringFiles);
        }

        [Benchmark]
        public object NativeDirectoryExists()
        {
            return Exists(DirectoryExistsNative, _existingDirectories);
        }

        [Benchmark]
        public object NativeFileExists()
        {
            return Exists(FileExistsNative, _existingFiles);
        }

        [Benchmark]
        public object NativeFileOrDirectoryExist()
        {
            return Exists(p => FileExistsNative(p) || DirectoryExistsNative(p), _existingFiles);
        }

        [Benchmark]
        public object NativeDirectoryDoesNotExist()
        {
            return Exists(DirectoryExistsNative, _nonExistingDirectories);
        }

        [Benchmark]
        public object NativeFileDoesNotExist()
        {
            return Exists(FileExistsNative, _nonExistringFiles);
        }

        [Benchmark]
        public object NativeFileOrDirectoryDoesNotExist()
        {
            return Exists(p => FileExistsNative(p) || DirectoryExistsNative(p), _nonExistringFiles);
        }
        
        [Benchmark]
        public object AttributesDirectoryExists()
        {
            return Exists(DirectoryExistsAttributes, _existingDirectories);
        }

        [Benchmark]
        public object AttributesFileExists()
        {
            return Exists(FileExistsAttributes, _existingFiles);
        }

        [Benchmark]
        public object AttributesFileOrDirectoryExist()
        {
            return Exists(p => FileExistsAttributes(p) || DirectoryExistsAttributes(p), _existingFiles);
        }

        [Benchmark]
        public object AttributesDirectoryDoesNotExist()
        {
            return Exists(DirectoryExistsAttributes, _nonExistingDirectories);
        }

        [Benchmark]
        public object AttributesFileDoesNotExist()
        {
            return Exists(FileExistsAttributes, _nonExistringFiles);
        }

        [Benchmark]
        public object AttributesFileOrDirectoryDoesNotExist()
        {
            return Exists(p => FileExistsAttributes(p) || DirectoryExistsAttributes(p), _nonExistringFiles);
        }
    }
}