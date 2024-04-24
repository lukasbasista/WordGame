using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WordGame.Helpers;

namespace WordGame.ViewModels
{

    internal class GameModeViewModel : ObservableObject
    {
        public ICommand SingleplayerModeCommand { get; }
        public ICommand MultiplayerModeCommand { get; }
        public ICommand GoBackCommand { get; }

        private readonly INavigationService _navigationService;
        public GameModeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SingleplayerModeCommand = new RelayCommand(OnSinglePlayer);
            MultiplayerModeCommand = new RelayCommand(OnMultiPlayer);
            GoBackCommand = new RelayCommand((object param) => _navigationService.GoBack());
        }

        private void OnSinglePlayer(object param)
        {
            _navigationService.NavigateTo("Game", true);
        }

        private void OnMultiPlayer(object param)
        {
            _navigationService.NavigateTo("Game", false);
        }
    }
}
