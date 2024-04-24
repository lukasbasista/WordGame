using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordGame.Helpers
{
    public class DatabaseManager
    {
        private SQLiteConnection dbConnection;

        public DatabaseManager(string dbPath)
        {
            dbConnection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            dbConnection.Open();
            CreateWordsTable();
        }

        private void CreateWordsTable()
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS words (
                word TEXT NOT NULL,
                initial CHAR(1) NOT NULL,
                final CHAR(1) NOT NULL
            );
        ";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        public void InsertWordsFromTSV(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.Split('\t');
                    if (fields.Length > 1)
                    {
                        string word = fields[1];
                        string initial = word.Substring(0, 1);
                        string final = word.Substring(word.Length - 1, 1);
                        InsertWord(word, initial, final);
                    }
                }
            }
        }

        private void InsertWord(string word, string initial, string final)
        {
            string sql = "INSERT INTO words (word, initial, final) VALUES (@word, @initial, @final)";
            using (var cmd = new SQLiteCommand(sql, dbConnection))
            {
                cmd.Parameters.AddWithValue("@word", word);
                cmd.Parameters.AddWithValue("@initial", initial);
                cmd.Parameters.AddWithValue("@final", final);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
