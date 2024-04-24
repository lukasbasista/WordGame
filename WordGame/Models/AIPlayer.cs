using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordGame.Models
{
    public class AIPlayer : Player
    {
        private SQLiteConnection _connection;

        public AIPlayer(string name) : base(name)
        {
            _connection = new SQLiteConnection("Data Source=database.db; Version=3;");
            _connection.Open();
        }

        public async Task<string> GenerateWordAsync(string lastWord, List<WordEntry> usedWords)
        {
            await Task.Delay(2000);
            return GenerateWord(lastWord, usedWords);
        }

        public string GenerateWord(string lastWord, List<WordEntry> usedWords)
        {
            char startChar = lastWord.Last();
            string wordsUsed = string.Join("', '", usedWords.Select(w => w.Word.Replace("'", "''")));
            string sql = $@"SELECT word FROM words 
                WHERE LOWER(initial) = LOWER('{startChar}') 
                AND word NOT IN ('{wordsUsed}')";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var words = new List<string>();
                    while (reader.Read())
                    {
                        words.Add(reader.GetString(0));
                    }
                    if (words.Count > 0)
                    {
                        Random random = new Random();
                        return words[random.Next(words.Count)];
                    }
                }
            }
            return null;
        }


        ~AIPlayer()
        {
            _connection.Close();
        }
    }
}
