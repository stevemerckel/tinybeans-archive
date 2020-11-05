using System;

namespace TBA.Common
{
    public class RuntimeSettings : IRuntimeSettings
    {
        private volatile bool _isInitialized;
        private readonly object _lock = new object();
        private string _jsonContent;

        public string AuthorizationHeaderKey
        {
            get => InitializeAndReturnValue(nameof(AuthorizationHeaderKey));
        }

        public string AuthorizationHeaderValue
        {
            get => InitializeAndReturnValue(nameof(AuthorizationHeaderValue));
        }

        public string ApiBaseUrl
        {
            get => InitializeAndReturnValue(nameof(ApiBaseUrl));
        }

        private string InitializeAndReturnValue(string propertyName)
        {
            var isFileReadSuccess = true; // important to init as true
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    // load from file: use try-catch --> if "catch", set isFileReadSuccess to false
                    // send contents to _jsonContent
                }
            }

            if (!isFileReadSuccess || string.IsNullOrWhiteSpace(_jsonContent))
            {
                throw new SettingsFailureException("General failure fetching the JSON content of runtime settings.");
            }


            // query and read from JsonContents for target propertyName

            throw new NotImplementedException();
        }
    }
}