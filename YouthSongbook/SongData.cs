using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            List<string> createTablesList = new List<string>();
            createTablesList.Add("CREATE TABLE ITEMS (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title ntext, Body ntext);");
            createTablesList.Add("CREATE TABLE CHORDS (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title ntext, Body ntext);");
            createTablesList.Add("CREATE TABLE UPDATENUM (Id INTEGER PRIMARY KEY AUTOINCREMENT, Number ntext);");
            createTablesList.Add("CREATE TABLE CHORDFLAG (Id INTEGER PRIMARY KEY AUTOINCREMENT, Flag ntext);");

            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                // Create the tables
                foreach (string sql in createTablesList)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                // Initialize chords database to be off
                string initChordsSQL = "INSERT INTO CHORDFLAG (Flag) VALUES (@Flag);";
                cmd.CommandText = initChordsSQL;
                cmd.Parameters.AddWithValue("@Flag", "0");
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        public static bool GetChords()
        {
            string chordsFlag = "0";

            // Create and open a database connection
            using (SqliteConnection connection = GetConnection())
            {
                connection.Open();

                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    // Read the chords table
                    string chordsSelect = "SELECT Flag FROM CHORDFLAG WHERE Id = 1;";
                    cmd.CommandText = chordsSelect;

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chordsFlag = reader.GetString(0);
                        }
                    }
                }
            }

            // Return the chords flag
            return chordsFlag.Equals("1");
        }

        public static void SetChords(bool chords)
        {
            using (SqliteConnection connection = GetConnection())
            {
                connection.Open();

                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    // Initialize chords database to be off
                    string flag = chords ? "1" : "0";
                    string chordsSQL = "UPDATE CHORDFLAG SET Flag = \"" + flag + "\" WHERE Id = 1;";
                    cmd.CommandText = chordsSQL;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static string[] GetAllTitles(bool chords)
        {
            List<string> titles = new List<string>();

            using (SqliteConnection conn = GetConnection())
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    string table = chords ? "CHORDS" : "ITEMS";
                    string sql = "SELECT Title FROM " + table + " ORDER BY Title ASC;";
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

        public static string GetSong(string title, bool chords)
        {   
            string song = string.Empty;

            using (SqliteConnection conn = GetConnection())
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    string table = chords ? "CHORDS" : "ITEMS";
                    string sql = "SELECT Body FROM " + table + " WHERE Title = \""
                        + title + "\";";
                    cmd.CommandText = sql;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            song = reader.GetString(0);
                        }
                    }
                }

                conn.Close();
            }

            return song;
        }
        
        public static void UpdateSongs(string updateNumber, Dictionary<string,string> dict, bool chords)
        {
            // Create and open a database connection
            using (SqliteConnection connection = GetConnection())
            {
                connection.Open();

                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    // Insert statements
                    string table = chords ? "CHORDS" : "ITEMS";
                    string sql = "INSERT INTO " + table + " (Title, Body) VALUES (@Title, @Body);";

                    // Incoming json values
                    foreach (string key in dict.Keys)
                    {
                        string value = dict[key];
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@Title", key);
                        cmd.Parameters.AddWithValue("@Body", value);
                        cmd.ExecuteNonQuery();
                    }

                    // Initialize update data
                    string sqlInitUpdate = "INSERT INTO UPDATENUM (Number) VALUES (@Number);";
                    cmd.CommandText = sqlInitUpdate;
                    cmd.Parameters.AddWithValue("@Number", updateNumber);
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static bool CheckUpdate(int number)
        {
            bool update = false;

            // Create and open a database connection
            using (SqliteConnection connection = GetConnection())
            { 
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

                        update = true;
                    }
                    else
                    {
                        update = false;
                    }
                }

                connection.Close();
            }

            return update;
        }

        public static void LoadDatabase(Stream asset, bool chords)
        {
            // Get the initial asset Json file
            string content;
            using (StreamReader sr = new StreamReader(asset))
            {
                content = sr.ReadToEnd();
            }

            UpdateSongs("0", SongNetwork.JsonToDictionary(JObject.Parse((content))), chords);
        }
    }
}