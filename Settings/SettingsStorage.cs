
namespace MonoSnake.Settings
{
    /// <summary>
    /// Provides a storage mechanism for game settings on desktop platforms.
    /// This class inherits from <see cref="BaseSettingsStorage"/> and initializes
    /// the storage path to the application's data folder.
    /// </summary>
    public class SettingsStorage : BaseSettingsStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSettingsStorage"/> class.
        /// Sets the storage path to the application's data folder (e.g., AppData on Windows).
        /// </summary>
        public SettingsStorage()
        {
            SpecialFolderPath = System.Environment.SpecialFolder.ApplicationData;
        }
    }
}
