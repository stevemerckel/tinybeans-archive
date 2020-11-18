namespace TBA.Common
{
    /// <summary>
    /// Provider for runtime settings
    /// </summary>
    public interface IRuntimeSettingsProvider
    {
        /// <summary>
        /// Returns a runtime settings collection object
        /// </summary>
        IRuntimeSettings GetRuntimeSettings();
    }
}