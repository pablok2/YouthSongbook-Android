using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace YouthSongbook
{
    public static class SongData
    {
        private static string db_file = "notes.db3";
        private static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);

        public static bool DataBaseExists { get { return File.Exists(dbPath); } }

        private static SqliteConnection GetConnection()
        {
            bool exists = DataBaseExists;

            if (!exists)
            {
                SqliteConnection.CreateFile(dbPath);
            }

            SqliteConnection conn = new SqliteConnection("Data Source=" + dbPath);

            if (!exists)
            {
                CreateDatabase(conn);
            }

            return conn;
        }

        private static void CreateDatabase(SqliteConnection connection)
        {
            // Create table statements
            string sql = "CREATE TABLE ITEMS (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title ntext, Body ntext);";
            string sqlUpdate = "CREATE TABLE UPDATENUM (Id INTEGER PRIMARY KEY AUTOINCREMENT, Number ntext);";

            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                // Create the tables
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sqlUpdate;
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public static string[] GetAllTitles()
        {
            string sql = "SELECT Title FROM ITEMS;";
            List<string> titles = new List<string>();
            using (SqliteConnection conn = GetConnection())
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            titles.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return titles.ToArray();
        }

        public static string GetSong(string title)
        {
            string sql = "SELECT Body FROM ITEMS WHERE Title = \""
                + title + "\";";

            string song = string.Empty;
            using (SqliteConnection conn = GetConnection())
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            song = reader.GetString(0);
                        }
                    }
                }
            }
            return song;
        }
        
        public static void UpdateSongs(string updateNumber, Dictionary<string,string> dict)
        {
            // Create and open a database connection
            SqliteConnection connection = GetConnection();
            connection.Open();
            
            // Insert statements
            string sql = "INSERT INTO ITEMS (Title, Body) VALUES (@Title, @Body);";
            string sqlInitUpdate = "INSERT INTO UPDATENUM (Number) VALUES (@Number);";

            // Incoming json values
            foreach (string key in dict.Keys)
            {
                string value = dict[key];
                    
                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Title", key);
                    cmd.Parameters.AddWithValue("@Body", value);
                    cmd.ExecuteNonQuery();
                }
            }

            // Update the update table
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                // Initialize update data
                cmd.CommandText = sqlInitUpdate;
                cmd.Parameters.AddWithValue("@Number", updateNumber);
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public static bool CheckUpdate(int number)
        {
            // Create and open a database connection
            SqliteConnection connection = GetConnection();
            connection.Open();

            int updateNumber = 0;
            string updateSelect = "SELECT Number FROM UPDATENUM WHERE Id = 1;";

            // Read the update table
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = updateSelect;

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string s = reader.GetString(0);
                        updateNumber = int.Parse(s);
                    }
                }

                // Update the table if need be
                if (updateNumber < number)
                {
                    string sqlInitUpdate = "UPDATE UPDATENUM SET Number = \"" +
                        (updateNumber + 1).ToString() +
                        "\" WHERE Id = 1;";
                    cmd.CommandText = sqlInitUpdate;
                    cmd.ExecuteNonQuery();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void LoadDatabase(Stream asset)
        {
            // Get the initial asset Json file
            string content;
            using (StreamReader sr = new StreamReader(asset))
            {
                content = sr.ReadToEnd();
            }

            UpdateSongs("0", SongNetwork.JsonToDictionary(JObject.Parse((content))));
        }
    }
}