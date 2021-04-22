using System.Reflection;
using Newtonsoft.Json;

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
        private readonly IFileManager _fileManager;

        public RuntimeSettingsProvider(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        /// <inheritdoc />
        public IRuntimeSettings GetRuntimeSettings()
        {
            lock (_lock)
            {
                if (_isInitialized)
                    return _runtimeSettings;

                // read and process file
                const string ExpectedFileName = "runtime.settings";
                var runtimeDirectory = _fileManager.DirectoryGetName(Assembly.GetExecutingAssembly().Location);
                var settingsLocation = _fileManager.PathCombine(runtimeDirectory, ExpectedFileName);
                if (!_fileManager.FileExists(settingsLocation))
                    throw new SettingsFailureException($"Could not find '{ExpectedFileName}' in '{runtimeDirectory}' !!");

                var fileContents = _fileManager.FileReadAllText(settingsLocation);
                if (string.IsNullOrWhiteSpace(fileContents))
                    throw new SettingsFailureException($"No content was found in file: {settingsLocation}");

                JsonSerializerSettings convertSettings = new JsonSerializerSettings() 
                { 
                    // todo: finish?
                };
                var rs = JsonConvert.DeserializeObject<RuntimeSettings>(fileContents, new RuntimeSettingsJsonConverter<RuntimeSettings>());
                _runtimeSettings = rs;
                _isInitialized = true;
            }

            return _runtimeSettings;
        }
    }
}