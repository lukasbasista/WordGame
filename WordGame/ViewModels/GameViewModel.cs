using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WordGame.Helpers;
using WordGame.Models;
using System.Windows.Media;
using System.Windows.Controls;

namespace WordGame.ViewModels
{
    public class GameViewModel : ObservableObject
    {
        private Game game;
        private string? currentPlayerName;
        private string? playerOneName;
        private string? playerTwoName;
        private string? currentWord;
        private string? remainingTime;
        public string ImageSourcePlayer1 { get; set; } = "/Assets/img/player.png";
        public string ImageSourcePlayer2 { get; set; } = "/Assets/img/player.png";
        private DispatcherTimer imageTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        private readonly INavigationService _navigationService;
        public ICommand GoBackCommand { get; private set; } = new RelayCommand(param => { });

        public ICommand AddWordCommand { get; private set; } = new RelayCommand(param => { });
        public ICommand MoveImageCommand { get; private set; } = new RelayCommand(param => { });
        public ICommand StartAnimationCommand { get; private set; } = new RelayCommand(param => { });
        public ICommand FocusInputCommand { get; private set; } = new RelayCommand(param => { });

        public ObservableCollection<string> PlayerOneWords { get; private set; } = new ObservableCollection<string>();
        public ObservableCollection<string> PlayerTwoWords { get; private set; } = new ObservableCollection<string>();

        public event Action? RequestStartAnimation;
        public event Action<bool>? RequestMoveImage;

        public string? CurrentPlayerName
        {
            get => currentPlayerName;
            set => SetProperty(ref currentPlayerName, value);
        }
        public string? PlayerOneName
        {
            get => playerOneName;
            set => SetProperty(ref playerOneName, value);
        }
        public string? PlayerTwoName
        {
            get => playerTwoName;
            set => SetProperty(ref playerTwoName, value);
        }
        public string? CurrentWord
        {
            get => currentWord;
            set => SetProperty(ref currentWord, value);
        }
        public string RemainingTime
        {
            get => remainingTime;
            set => SetProperty(ref remainingTime, value);
        }

        private bool CanAddWord(object parameter)
        {
            return !game.GameOver && !(game.CurrentPlayer is AIPlayer);
        }

        private bool _isInputEnabled = true;
        public bool IsInputEnabled
        {
            get => _isInputEnabled;
            set
            {
                if (_isInputEnabled != value)
                {
                    _isInputEnabled = value;
                    OnPropertyChanged(nameof(IsInputEnabled));
                }
            }
        }

        public GameViewModel(INavigationService navigationService, bool playVsComputer)
        {
            _navigationService = navigationService;
            game = new Game("Player 1", playVsComputer ? "Computer" : "Player 2", playVsComputer);
            game.OnGameEnded += Game_OnGameEnded;
            game.OnAiMoveCompleted += HandleAiMoveCompleted;
            PlayerOneName = game.PlayerOne.Name;
            PlayerTwoName = game.PlayerTwo.Name;

            InitializeUI();
            InitializeCommands();
            SetupTimers();
        }

        private void SetupTimers()
        {
            imageTimer.Interval = TimeSpan.FromMilliseconds(250);
            imageTimer.Tick += ImageTimer_Tick;
            imageTimer.Start();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void InitializeCommands()
        {
            AddWordCommand = new RelayCommand(AddWord, CanAddWord);
            StartAnimationCommand = new RelayCommand(ExecuteStartAnimation);
            MoveImageCommand = new RelayCommand(ExecuteMoveImage);
            GoBackCommand = new RelayCommand(param =>
            {
                Dispose();
                _navigationService.GoBack();
            });
            FocusInputCommand = new RelayCommand(FocusInput);
        }

        private void InitializeUI()
        {
            InitializePlayerWordLists();
            UpdateUI();
        }

        private async void HandleAiMoveCompleted()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AddWord(new Object());
            }));
        }

        private void FocusInput(object parameter)
        {
            Window activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            TextBox textBox = activeWindow?.FindName("TextInput") as TextBox;
            textBox?.Focus();
        }

        private void InitializePlayerWordLists()
        {
            UpdateWordLists();
        }

        private void Game_OnGameEnded(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message);
                ResetGameUI();
            });
        }

        private void ResetGameUI()
        {
            CurrentWord = string.Empty;
            IsInputEnabled = true;
            UpdateWordLists();
            RemainingTime = $"Time left: {game.TurnTimeLimit}s";
            game.ResetGame();
            OnPropertyChanged(nameof(CurrentPlayerName));
        }

        // TODO: implement start new game
        public void StartNewGame()
        {
            if (!game.GameOver)
            {
                var result = MessageBox.Show("A game is currently in progress. Do you want to start a new game?", "Confirm New Game", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    game.EndGame();
                }
            }
            else
            {
                game.StartGame();
            }
        }

        private async void AddWord(object parameter)
        {
            if (game.CurrentPlayer is AIPlayer aiPlayer)
            {
                Console.WriteLine("AI Turn");
                IsInputEnabled = false;
                string aiWord = await aiPlayer.GenerateWordAsync(game.UsedWords.LastOrDefault()?.Word ?? "", game.UsedWords);
                Console.WriteLine("Simulating");
                if (!string.IsNullOrEmpty(aiWord))
                {
                    await SimulateTyping(aiWord);
                    Console.WriteLine("Adding word");
                    bool added = await game.AddWord(aiWord);
                    if (added)
                    {
                        Console.WriteLine("Word added");
                        UpdateAfterWordAdded();
                    }
                }
                else
                {
                    Console.WriteLine("AI was unable to generate a word. Ending the game.");
                    game.EndGame();
                }
                await Task.Delay(100);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsInputEnabled = true;
                    FocusInput(new object());
                    UpdateUI();
                });
                await game.SwitchPlayerAsync();
            }
            else if (!string.IsNullOrEmpty(CurrentWord))
            {
                Console.WriteLine("addword");
                bool added = await game.AddWord(CurrentWord);
                if (added)
                {
                    Console.WriteLine("Word Added");
                    UpdateAfterWordAdded();
                    await game.SwitchPlayerAsync();
                }
                else
                {
                    Console.WriteLine("Invalid word or game logic prevented adding the word.");
                }
            }
            UpdateUI();
        }

        private void UpdateAfterWordAdded()
        {
            SoundManager.PlayKickSound();
            UpdateImageSource(game.CurrentPlayer == game.PlayerOne);
            UpdateUI();
            CurrentWord = "";
            UpdateWordLists();
        }
        private void UpdateImageSource(bool isPlayerTwo)
        {
            if (isPlayerTwo)
            {
                ImageSourcePlayer1 = "/Assets/img/player1.png";
                OnPropertyChanged(nameof(ImageSourcePlayer1));
                RequestMoveImage?.Invoke(true);
            }
            else
            {
                ImageSourcePlayer2 = "/Assets/img/player1.png";
                OnPropertyChanged(nameof(ImageSourcePlayer2));
                RequestMoveImage?.Invoke(false);
            }
            imageTimer.Start();
        }

        private void ImageTimer_Tick(object? sender, EventArgs? e)
        {
            ImageSourcePlayer1 = "/Assets/img/player.png";
            ImageSourcePlayer2 = "/Assets/img/player.png";
            imageTimer.Stop();
            OnPropertyChanged(nameof(ImageSourcePlayer1));
            OnPropertyChanged(nameof(ImageSourcePlayer2));
        }

        private void Timer_Tick(object? sender, EventArgs? e)
        {
            int remaining = game.RemainingSeconds;
            if (remaining > 0)
            {
                RemainingTime = $"Time left: {remaining}s";
                OnPropertyChanged(nameof(RemainingTime));
            }
        }

        private void UpdateWordLists()
        {
            PlayerOneWords = new ObservableCollection<string>(game.UsedWords.Where(w => w.Player == game.PlayerOne).Select(w => w.Word).Reverse());
            PlayerTwoWords = new ObservableCollection<string>(game.UsedWords.Where(w => w.Player == game.PlayerTwo).Select(w => w.Word).Reverse());
            OnPropertyChanged(nameof(PlayerOneWords));
            OnPropertyChanged(nameof(PlayerTwoWords));
        }

        private void UpdateUI()
        {
            CurrentPlayerName = game.CurrentPlayer.Name;
        }

        private void ExecuteStartAnimation(object parameter)
        {
            RequestStartAnimation?.Invoke();
        }

        public bool isForward = true;
        private void ExecuteMoveImage(object parameter)
        {
            RequestMoveImage?.Invoke(isForward);
            isForward = !isForward;
        }
        private async Task SimulateTyping(string word)
        {
            Random random = new Random();
            StringBuilder typedWord = new StringBuilder();

            foreach (char c in word)
            {
                int delay = random.Next(10, 500);
                await Task.Delay(delay);

                if (random.Next(0, 10) == 0)
                {
                    char wrongChar = (char)random.Next(97, 123);
                    typedWord.Append(wrongChar);
                    CurrentWord = typedWord.ToString();
                    OnPropertyChanged(nameof(CurrentWord));
                    await Task.Delay(random.Next(10, 500));
                    typedWord.Remove(typedWord.Length - 1, 1);
                    CurrentWord = typedWord.ToString();
                    OnPropertyChanged(nameof(CurrentWord));
                }

                typedWord.Append(c);
                CurrentWord = typedWord.ToString();
                OnPropertyChanged(nameof(CurrentWord));
            }
        }
        public void Dispose()
        {
            game.EndGame();
            timer.Stop();
            imageTimer.Stop();
        }
    }
}

