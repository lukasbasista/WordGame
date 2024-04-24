using System;
using System.Windows;
using System.Windows.Input;
using WordGame.Helpers;
using WordGame.Models;
using WordGame.Views;

namespace WordGame.ViewModels
{
    public class MainMenuViewModel : ObservableObject
    {

        public ICommand NewGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand SettingsCommand { get; }

        private readonly INavigationService _navigationService;

        public MainMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NewGameCommand = new RelayCommand(OnNewGame);
            LoadGameCommand = new RelayCommand(OnLoadGame);
            SettingsCommand = new RelayCommand(OnSettings);

        }

        private void OnNewGame(object param)
        {
            _navigationService.NavigateTo("GameMode");
        }

        private void OnLoadGame(object param)
        {
            // TODO: Implement
        }

        private void OnSettings(object param)
        {
            _navigationService.NavigateTo("Settings");
        }




    }
}