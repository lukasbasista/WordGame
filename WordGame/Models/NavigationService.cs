using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WordGame.Helpers;
using WordGame.ViewModels;
using WordGame.Views;
using System.Windows;

namespace WordGame.Models
{
    public class NavigationService : INavigationService
    {
        private Stack<UserControl> navigationStack = new Stack<UserControl>();

        public void NavigateTo(string viewName, object parameter = null)
        {
            DisposeCurrentView();
            UserControl view = null;
            switch (viewName)
            {
                case "MainMenu":
                    view = new MainMenuView { DataContext = new MainMenuViewModel(this) };
                    break;
                case "Game":
                    bool playVsComputer = parameter as bool? ?? false;
                    view = new GameView { DataContext = new GameViewModel(this, playVsComputer) };
                    break;
                case "GameMode":
                    view = new GameModeView { DataContext = new GameModeViewModel(this) };
                    break;
                case "Settings":
                    view = new SettingsView { DataContext = new SettingsViewModel(this) };
                    break;
            }

            if (view != null)
            {
                navigationStack.Push(view);
                UpdateCurrentView(view);
            }
        }

        public void GoBack()
        {
            if (navigationStack.Count > 1)
            {
                navigationStack.Pop();
                UserControl previousView = navigationStack.Peek();
                UpdateCurrentView(previousView);
            }
        }

        private void UpdateCurrentView(UserControl view)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainContentControl.Content = view;
            }
        }

        private void DisposeCurrentView()
        {
            if (navigationStack.Any())
            {
                var currentView = navigationStack.Peek();
                if (currentView.DataContext is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
