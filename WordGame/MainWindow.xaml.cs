using System.Windows;
using WordGame.Models;

namespace WordGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var navigationService = new NavigationService();
            navigationService.NavigateTo("MainMenu");
        }
    }
}
