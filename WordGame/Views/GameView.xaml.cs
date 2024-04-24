using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WordGame.ViewModels;

namespace WordGame.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
            Loaded += GameView_Loaded;
            Unloaded += GameView_Unloaded;
        }

        private void GameView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.RequestMoveImage += ViewModel_RequestMoveImage;
            }
        }

        private void GameView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.RequestMoveImage -= ViewModel_RequestMoveImage;
            }
        }

        private void ViewModel_RequestMoveImage(bool isForward)
        {
            string storyboardName = isForward ? "ForwardAnimation" : "ReverseAnimation";
            Storyboard storyboard = Resources[storyboardName] as Storyboard;
            if (storyboard != null)
            {
                Storyboard.SetTarget(storyboard, AnimatedImage);
                storyboard.Begin();
            }
        }
    }
}
