using IfeelgameFramework.Core.Storage;

namespace IfeelgameFramework.Core.Sound
{
    public class SoundSettings
    {
        private const string KeyLocalStorageName = "SoundSettings";
        private const string KeyMusicEnabled = "MusicEnabled";
        private const string KeySoundEnabled = "SoundEnabled";
        private const string KeyMusicVolume = "MusicVolume";
        private const string KeySoundVolume = "SoundVolume";

        private float _musicVolume;
        private float _soundVolume;
        private bool _musicEnabled;
        private bool _soundEnabled;
        private readonly LocalStorage _storage;

        public SoundSettings()
        {
            _storage = LocalStorageManager.Instance.GetLocalStorage(KeyLocalStorageName);
            
            _musicEnabled = _storage.GetValue(KeyMusicEnabled, true);
            _soundEnabled = _storage.GetValue(KeySoundEnabled, true);
            _musicVolume = _storage.GetValue(KeyMusicVolume, 1f);
            _soundVolume = _storage.GetValue(KeySoundVolume, 1f);
        }

        ~SoundSettings()
        {
            _storage.Dispose();
        }

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                _storage.SetValue(KeyMusicEnabled, _musicEnabled, true, true);
            }
        }

        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                _soundEnabled = value;
                _storage.SetValue(KeySoundEnabled, _soundEnabled, true, true);
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                _storage.SetValue(KeyMusicVolume, _musicVolume, true, true);
            }
        }

        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = value;
                _storage.SetValue(KeySoundVolume, _soundVolume, true, true);
            }
        }
    }
}
