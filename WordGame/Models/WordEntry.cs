namespace WordGame.Models
{
    public class WordEntry
    {
        public string Word { get; private set; }
        public Player Player { get; private set; }

        public WordEntry(string word, Player player)
        {
            Word = word;
            Player = player;
        }
    }
}
