﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Trinet.Core.IO.Ntfs;

namespace TBA.Common
{
    /// <inheritdoc />
    public sealed class WindowsFileSystemManager : IFileManager
    {
        /// <inheritdoc />
        public void FileDelete(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "Value received was null/empty!");

            try
            {
                File.SetAttributes(fileLocation, FileAttributes.Normal);
                File.Delete(fileLocation);
            }
            catch (IOException)
            {
                Thread.Sleep(2);
                File.Delete(fileLocation);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(2);
                File.Delete(fileLocation);
            }
        }

        /// <inheritdoc />
        public bool FileExists(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "Value received was null/empty!");

            return File.Exists(fileLocation);
        }

        /// <inheritdoc />
        public string FileHash(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "File location is empty/null!");

            if (!FileExists(fileLocation))
                throw new FileNotFoundException("File could not be found!", fileLocation);

            using (MD5 md5Check = new MD5CryptoServiceProvider())
            {
                const int BufferSize = 1200000;
                using (var localStream = new BufferedStream(File.OpenRead(fileLocation), BufferSize))
                {
                    var hash = md5Check.ComputeHash(localStream);
                    return MakeHexString(hash);
                }
            }
        }

        /// <inheritdoc />
        public void FileUnblock(string fileLocation)
        {
            if (!FileExists(fileLocation))
                throw new FileNotFoundException("Cannot unblock a file that cannot be found!");

            const string AlternateDataStreamNameToRemove = "Zone.Identifier";
            new FileInfo(fileLocation).DeleteAlternateDataStream(AlternateDataStreamNameToRemove);
        }

        /// <inheritdoc />
        public string FileReadAllText(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "Value received was null/empty!");

            return File.ReadAllText(fileLocation.Trim());
        }

        /// <inheritdoc />
        public bool DirectoryExists(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "Value cannot be null or empty.");

            return Directory.Exists(fileLocation.Trim());
        }

        /// <inheritdoc />
        public void FileWriteText(string fileLocation, string contents)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "Value cannot be null or empty.");

            ConsiderDestinationDirectoryFolderCreation(fileLocation);

            try
            {
                File.WriteAllText(fileLocation, contents);
            }
            catch (IOException)
            {
                Thread.Sleep(2);
                File.WriteAllText(fileLocation, contents);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(2);
                File.WriteAllText(fileLocation, contents);
            }
        }

        /// <inheritdoc />
        public void FileWriteBytes(string fileLocation, byte[] bits)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentNullException(nameof(fileLocation), "Value cannot be null or empty.");

            ConsiderDestinationDirectoryFolderCreation(fileLocation);

            try
            {
                File.WriteAllBytes(fileLocation, bits);
            }
            catch (IOException)
            {
                Thread.Sleep(2);
                File.WriteAllBytes(fileLocation, bits);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(2);
                File.WriteAllBytes(fileLocation, bits);
            }
        }

        /// <inheritdoc />
        public void CreateDirectory(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentNullException(nameof(directoryPath), "Value received was null/empty!");

            if (!DirectoryExists(directoryPath))
                Directory.CreateDirectory(directoryPath.Trim());
        }

        /// <summary>
        /// Looks at the received file path (which may not yet exist!) and tests whether the folder structure exists.  If not, then it tries to create the folder structure.
        /// </summary>
        /// <param name="filePathToConsider">The file location to consider</param>
        private void ConsiderDestinationDirectoryFolderCreation(string filePathToConsider)
        {
            if (string.IsNullOrWhiteSpace(filePathToConsider))
                return; // we got nothing, so exiting and letting caller handle any null/empty/whitespace issues

            var destinationDirectory = Path.GetDirectoryName(filePathToConsider.Trim());

            if (DirectoryExists(destinationDirectory))
                return; // already exists!

            // Get directory parts
            var separator = Path.DirectorySeparatorChar;
            var parts = destinationDirectory.Split(new[] { separator }, StringSplitOptions.None);

            // Find out how far back from the right-most part of path that we need to start from.
            // NOTE: We going right-to-left instead of left-to-right because account permissions 
            //       or network share roots may throw access exceptions if we start from the left-most piece.
            var startIndex = parts.Length - 2; // testing the second-from-the-right, since earlier "if" tested the full folder path
            var root = string.Empty;
            while (startIndex > 0)
            {
                var testMe = string.Join(separator.ToString(), parts, 0, startIndex + 1);
                if (DirectoryExists(testMe))
                {
                    // we found the known starting point!
                    root = testMe;
                    break;
                }

                startIndex--;
            }

            for (var i = 1; i < parts.Length - startIndex; i++)
            {
                var makeMe = root + separator + string.Join(separator.ToString(), parts, startIndex + 1, i);
                CreateDirectory(makeMe);
                Thread.Sleep(2);
            }
        }

        /// <summary>
        /// Returns a hexadecimal string representation of the byte array.
        /// </summary>
        /// <param name="bits">The byte array</param>
        /// <returns>Hexadecimal string</returns>
        private static string MakeHexString(byte[] bits)
        {
            var sb = new StringBuilder(bits.Length * 2); // pre-fill the size to prevent potential re-allocation of memory from dynamic re-sizing of the object
            bits.ToList().ForEach(x => sb.Append(x.ToString("X2")));
            return sb.ToString();
        }
    }
}