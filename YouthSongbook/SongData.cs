#region

using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;

#endregion

namespace YouthSongbook
{
    internal enum Setting
    {
        Chords,
        Contrast,
        UpdateFlag
    }

    internal static class SongData
    {
        private static readonly string db_file = "notes.db3";

        private static readonly string dbPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db_file);

        internal static bool DataBaseExists
        {
            get { return File.Exists(dbPath); }
        }

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
            string[] createTablesList =
            {
                "CREATE TABLE ITEMS (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title ntext, Body ntext);",
                "CREATE TABLE CHORDS (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title ntext, Body ntext);",
                "CREATE TABLE UPDATENUM (Id INTEGER PRIMARY KEY AUTOINCREMENT, Number ntext);",
                "CREATE TABLE CHORDFLAG (Id INTEGER PRIMARY KEY AUTOINCREMENT, Flag ntext);",
                "CREATE TABLE CONTRAST (Id INTEGER PRIMARY KEY AUTOINCREMENT, Flag ntext);",
                "CREATE TABLE UPDATEFLAG (Id INTEGER PRIMARY KEY AUTOINCREMENT, Flag ntext);"
            };

            connection.Open();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                // Create the tables
                foreach (string sql in createTablesList)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                // Initialize database flags to be off
                string[] initFlagSQL = 
                {
                    "INSERT INTO CHORDFLAG (Flag) VALUES (@Flag);",
                    "INSERT INTO UPDATEFLAG (Flag) VALUES (@Flag);",
                    "INSERT INTO CONTRAST (Flag) VALUES (@Flag);"
                };

                foreach (string sqlLine in initFlagSQL)
                {
                    cmd.CommandText = sqlLine;
                    cmd.Parameters.AddWithValue("@Flag", "0");
                    cmd.ExecuteNonQuery();
                }
            }

            connection.Close();
        }

        internal static bool GetSetting(Setting setting)
        {
            string table;
            string flag = "0";

            switch (setting)
            {
                case Setting.Chords:
                    table = "CHORDFLAG";
                    break;
                case Setting.Contrast:
                    table = "CONTRAST";
                    break;
                case Setting.UpdateFlag:
                    table = "UPDATEFLAG";
                    break;
                default:
                    return false;
            }

            // Create and open a database connection
            using (SqliteConnection connection = GetConnection())
            {
                connection.Open();

                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    // Read the chords table
                    cmd.CommandText = "SELECT Flag FROM " + table + " WHERE Id = 1;";

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            flag = reader.GetString(0);
                        }
                    }
                }
            }

            // Return the chords flag
            return flag.Equals("1");
        }

        internal static void SetSetting(Setting setting, bool flag)
        {
            string table = string.Empty;

            switch (setting)
            {
                case Setting.Chords:
                    table = "CHORDFLAG";
                    break;
                case Setting.Contrast:
                    table = "CONTRAST";
                    break;
                case Setting.UpdateFlag:
                    table = "UPDATEFLAG";
                    break;
            }

            using (SqliteConnection connection = GetConnection())
            {
                connection.Open();

                using (SqliteCommand cmd = connection.CreateCommand())
                {
                    // Initialize chords database to be off
                    string value = flag ? "1" : "0";
                    string chordsSQL = "UPDATE " + table + " SET Flag = \"" + value + "\" WHERE Id = 1;";
                    cmd.CommandText = chordsSQL;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        internal static string[] GetAllTitles(bool chords)
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

        internal static string GetSong(string title, bool chords)
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

        internal static void UpdateSongs(string updateNumber, Dictionary<string, string> dict, bool chords)
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

        internal static bool CheckUpdate(int number)
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
                                               (updateNumber + 1) +
                                               "\" WHERE Id = 1;";
                        cmd.CommandText = sqlInitUpdate;
                        cmd.ExecuteNonQuery();

                        update = true;
                    }
                }

                connection.Close();
            }

            return update;
        }

        internal static void LoadDatabase(Stream asset, bool chords)
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