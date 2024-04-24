using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WordGame.Helpers;

namespace WordGame.ViewModels
{
    internal class SettingsViewModel : ObservableObject
    {
        public ICommand GoBackCommand { get; }

        private readonly INavigationService _navigationService;

        public int _playTime;

        public int PlayTime
        {
            get => Properties.Settings.Default.PlayTime;
            set
            {
                if (Properties.Settings.Default.PlayTime != value)
                {
                    Properties.Settings.Default.PlayTime = value;
                    Properties.Settings.Default.Save();
                    _playTime = value;
                    OnPropertyChanged(nameof(PlayTime));
                }
            }
        }

        public double SoundtrackVolume
        {
            get => Properties.Settings.Default.SoundtrackVolume;
            set
            {
                if (Properties.Settings.Default.SoundtrackVolume != value)
                {
                    Properties.Settings.Default.SoundtrackVolume = value;
                    Properties.Settings.Default.Save();
                    App.MediaPlayer.Volume = value;
                    OnPropertyChanged(nameof(SoundtrackVolume));
                }
            }
        }

        private double _effectVolume;
        public double EffectVolume
        {
            get => _effectVolume;
            set
            {
                if (SetProperty(ref _effectVolume, value))
                {
                    Properties.Settings.Default.SoundEffectVolume = value;
                    Properties.Settings.Default.Save();
                }
            }
        }
        public SettingsViewModel(INavigationService navigationService)
        {
            _effectVolume = Properties.Settings.Default.SoundEffectVolume;
            _navigationService = navigationService;
            GoBackCommand = new RelayCommand((object param) => _navigationService.GoBack());
            SoundtrackVolume = Properties.Settings.Default.SoundtrackVolume;
            PlayTime = Properties.Settings.Default.PlayTime;
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);
        }

        public ICommand ChangeLanguageCommand { get; }

        private void ChangeLanguage(object parameter)
        {
            if (parameter is string cultureCode)
            {
                ChangeLanguageResourceDictionary(cultureCode);
            }
        }

        private void ChangeLanguageResourceDictionary(string cultureCode)
        {
            var oldDict = Application.Current.Resources.MergedDictionaries
                          .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("pack://application:,,,/Resources/Strings."));

            if (oldDict != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
            }

            var newDict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Resources/Strings.{cultureCode}.xaml")
            };
            Application.Current.Resources.MergedDictionaries.Add(newDict);
        }


    }
}

