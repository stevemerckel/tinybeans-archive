namespace TBA.Common
{
    /// <summary>
    /// Methods and functions for working with file systems
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Creates a file at the specified location and writes the text received.  If the file exists, it will be overwritten.
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        /// <param name="contents">The text contents to write</param>
        void FileWriteText(string fileLocation, string contents);

        /// <summary>
        /// Creates a file at the specified location and writes the bits received.  If the file exists, it will be overwritten.
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        /// <param name="bits">The bits to write to the file</param>
        void FileWriteBytes(string fileLocation, byte[] bits);

        /// <summary>
        /// Whether or not the specified file exists
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        /// <returns>Boolean indicating whether the file is found (<c>true</c>) or not found (<c>false</c>)</returns>
        bool FileExists(string fileLocation);

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        void FileDelete(string fileLocation);

        /// <summary>
        /// Generates a hexadecimal hash of the file contents
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        /// <returns>The hash as a hexadecimal string</returns>
        string FileHash(string fileLocation);
        
        /// <summary>
        /// Unblocks the file, which usually is blocked for files pulled from web/network paths
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        void FileUnblock(string fileLocation);

        /// <summary>
        /// Returns the text contents of a file
        /// </summary>
        /// <param name="fileLocation">The file path</param>
        /// <returns>The file's text contents</returns>
        string FileReadAllText(string fileLocation);

        /// <summary>
        /// Checks to see if a directory exists at the specified path.
        /// </summary>
        /// <param name="fileLocation">The path to evaluate</param>
        bool DirectoryExists(string fileLocation);

        /// <summary>
        /// Creates directory in the specified path unless they already exist.
        /// </summary>
        /// <param name="directoryPath">The directory to create.</param>
        void CreateDirectory(string directoryPath);
    }
}