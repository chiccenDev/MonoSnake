using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSnake.Settings
{
    /// <summary>
    /// Manages application settings using a specified storage implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SettingsManager<T> where T : new()
    {
        private readonly ISettingsStorage storage;

        /// <summary>
        /// Provides access to the underlying settings storage mechanism.
        /// </summary>
        public ISettingsStorage Storage => storage;

        private T settings;
        /// <summary>
        /// Provides access to the currently loaded settings. Will never be null.
        /// </summary>
        public T Settings => settings;

        /// <summary>
        /// Occurs when settings are successfully loaded.
        /// </summary>
        public event Action<T> SettingsLoaded;

        /// <summary>
        /// Occurs when settings are successfully saved.
        /// </summary>
        public event Action<T> SettingsSaved;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager{t}"/> class.
        /// </summary>
        /// <param name="storage">the storage implementation to handle saving and loading settings.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SettingsManager(ISettingsStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Load();
        }

        /// <summary>
        /// saves the current settings to the specified storage.
        /// </summary>
        public void Save()
        {
            try
            {
                storage.SaveSettings(settings);
                SettingsSaved?.Invoke(settings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save settings: {ex}");
            }
        }

        /// <summary>
        /// Loads settings from the specified storage. if loading fails, initializes with default values.
        /// </summary>
        public void Load()
        {
            try
            {
                settings = storage.LoadSettings<T>() ?? new T();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load settings, initializing defaults: {ex}");
                settings = new T();
            }

            SettingsLoaded?.Invoke(settings);
        }
    }
}
