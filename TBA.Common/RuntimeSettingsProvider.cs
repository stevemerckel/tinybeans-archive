using System;
using System.IO;
using System.Reflection;

namespace TBA.Common
{
    /// <summary>
    /// Implementation of <see cref="IRuntimeSettingsProvider"/>
    /// </summary>
    public sealed class RuntimeSettingsProvider : IRuntimeSettingsProvider
    {
        private bool _isInitialized;
        private readonly object _lock = new object();
        private IRuntimeSettings _runtimeSettings;

        /// <inheritdoc />
        public IRuntimeSettings Get()
        {
            lock (_lock)
            {
                if (_isInitialized)
                    return _runtimeSettings;

                // read and process file
                const string ExpectedFileName = "runtime.settings";
                var runtimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var settingsLocation = Path.Combine(runtimeDirectory, ExpectedFileName);
                if (!File.Exists(settingsLocation))
                    throw new FileNotFoundException($"Could not find '{ExpectedFileName}' in '{runtimeDirectory}' !!");

                var fileContents = File.ReadAllText(settingsLocation);
                if (string.IsNullOrWhiteSpace(fileContents))
                    throw new SettingsFailureException($"No content was found in file: {settingsLocation}");


                _isInitialized = true;
                throw new NotImplementedException();
            }

        }
    }
}