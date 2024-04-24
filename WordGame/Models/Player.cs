using System;

namespace WordGame.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; private set; }
        public bool IsTurn { get; set; }

        public Player(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Player name cannot be empty.", nameof(name));
            }

            Name = name;
            Score = 0;
            IsTurn = false;
        }

        public void IncrementScore()
        {
            Score++;
        }

        public void SetTurn(bool turn)
        {
            IsTurn = turn;
        }

        public void ResetScore()
        {
            Score = 0;
        }

        public override string ToString()
        {
            return $"{Name} - Skóre: {Score}";
        }
    }
}
