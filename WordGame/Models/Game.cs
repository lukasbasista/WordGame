using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WordGame.Models
{
    public class Game
    {
        public Player PlayerOne { get; private set; }
        public Player PlayerTwo { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public int TurnTimeLimit = Properties.Settings.Default.PlayTime;

        public bool GameOver { get; private set; }
        private bool isVsComputer;

        public List<WordEntry> UsedWords { get; private set; }

        public event Action OnAiMoveCompleted;
        public event Action<string> OnGameEnded;

        private Timer gameTimer;
        private DateTime endTime;


        public int RemainingSeconds
        {
            get
            {
                if (gameTimer.Enabled)
                {
                    TimeSpan remaining = endTime - DateTime.Now;
                    return remaining.TotalSeconds > 0 ? (int)remaining.TotalSeconds : 0;
                }
                return TurnTimeLimit;
            }
        }

        public Game(string playerOneName, string playerTwoName, bool vsComputer)
        {
            try
            {
                PlayerOne = new Player(playerOneName);
                PlayerTwo = vsComputer ? new AIPlayer("Computer") : new Player(playerTwoName);
                isVsComputer = vsComputer;
                UsedWords = new List<WordEntry>();
                GameOver = false;
                CurrentPlayer = PlayerOne;
                SetupTimer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while initializing the game: {ex.Message}");
                throw;
            }
        }

        private void SetupTimer()
        {
            gameTimer = new Timer(TurnTimeLimit * 1000);
            gameTimer.Elapsed += HandleTimeOut;
            gameTimer.AutoReset = false;
        }


        public void StartGame()
        {
            GameOver = false;
            UsedWords.Clear();
            PlayerOne.ResetScore();
            PlayerTwo.ResetScore();
            CurrentPlayer = PlayerOne;
            endTime = DateTime.Now.AddSeconds(TurnTimeLimit);
            gameTimer.Start();
            Console.WriteLine("Game started! It's " + CurrentPlayer.Name + "'s turn.");
        }

        public Task<bool> AddWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return Task.FromResult(false);

            if (UsedWords.Any() && !word.StartsWith(UsedWords.Last().Word.Last().ToString(), StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            UsedWords.Add(new WordEntry(word, CurrentPlayer));
            CurrentPlayer.IncrementScore();

            RestartTimer();
            return Task.FromResult(true);
        }

        private void RestartTimer()
        {
            gameTimer.Stop();
            endTime = DateTime.Now.AddSeconds(TurnTimeLimit);
            gameTimer.Start();
        }

        public Task SwitchPlayerAsync()
        {
            CurrentPlayer = CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;
            Console.WriteLine("Player now playing: " + CurrentPlayer.Name);

            if (isVsComputer && CurrentPlayer is AIPlayer aiPlayer)
            {
                OnAiMoveCompleted?.Invoke();
            }
            else
            {
                RestartTimer();
            }

            return Task.CompletedTask;
        }

        private void HandleTimeOut(object sender, ElapsedEventArgs e)
        {
            gameTimer.Stop();
            GameOver = true;
            Console.WriteLine($"Time is up! {CurrentPlayer.Name} has lost the game.");
            EndGame();
        }

        public void EndGame()
        {
            GameOver = true;
            gameTimer.Stop();
            OnGameEnded?.Invoke("The game has ended. The winner is " + (CurrentPlayer == PlayerOne ? PlayerTwo.Name : PlayerOne.Name));

            ResetGame();
        }


        public void ResetGame()
        {
            UsedWords.Clear();
            PlayerOne.ResetScore();
            PlayerTwo.ResetScore();
            CurrentPlayer = PlayerOne;
            GameOver = false;
        }
    }
}
