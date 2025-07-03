using System.ComponentModel;
using System.Runtime.CompilerServices;
using MonoSnake.Effects;

namespace MonoSnake.Settings
{
    /// <summary>
    /// Represents the game settings, including display, language, and particle effects.
    /// This class implements <see cref="INotifyPropertyChanged"/> to notify subscribers
    /// when a property value changes, enabling data binding and UI updates.
    /// </summary>
    public class GameSettings : INotifyPropertyChanged
    {
        private bool fullScreen;
        private int language = 2; // Default to English
        private ParticleEffectType particleEffect;

        /// <summary>
        /// Gets or sets whether the game is in full-screen mode.
        /// </summary>
        /// <value>
        /// <c>true</c> is the game is in full-screen mode; otherwise, <c>false</c>.
        /// </value>
        public bool FullScreen
        {
            get => fullScreen;
            set
            {
                if (fullScreen != value)
                {
                    fullScreen = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the language settings for the game.
        /// </summary>
        /// <value>
        /// An integer representing the selected language. The value corresponds to a language
        /// option in the game's localization system.
        /// </value>
        public int Language
        {
            get => language;
            set
            {
                if (language != value)
                {
                    language = value;
                    OnPropertyChanged();
                }
            }
        }

        public ParticleEffectType ParticleEffect
        {
            get => particleEffect;
            set
            {
                if (particleEffect != value)
                {
                    particleEffect = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
