using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace YouthSongbook
{
    public static class SongNetwork
    {
        private static async Task<JObject> FetchDataAsync(string url)
        {
            //// Create an HTTP web request using the URL:
            var request = WebRequest.Create(url);
            var response = (HttpWebResponse)await Task.Factory
                .FromAsync<WebResponse>(request.BeginGetResponse,
                                        request.EndGetResponse,
                                        null);

            // Get a stream representation of the HTTP web response:
            using (Stream stream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                // Read the Stream and parse the json
                JObject json = JObject.Parse(await Task.Run(() => sr.ReadToEndAsync()));
                return json;
            }
        }

        public static async Task<int> PerformUpdateAsync()
        {
            // The URL strings to get json objects from
            string baseURL = "http://frbcme.org/test/";
            string updateJsonListURL = baseURL + "updatelist.json";

            Dictionary<string, string> updates = JsonToDictionary(await FetchDataAsync(updateJsonListURL));

            foreach (string updateNumber in updates.Keys)
            {
                // Check the current update
                bool updateValid = SongData.CheckUpdate(int.Parse(updateNumber));

                if (updateValid)
                {
                    string jsonFileName = updates[updateNumber];
                    string updateJsonURL = baseURL + jsonFileName;

                    if (jsonFileName.Contains("chords"))
                    {
                        SongData.UpdateSongs(updateNumber, JsonToDictionary(await FetchDataAsync(updateJsonURL)), true);
                    }
                    else
                    {
                        SongData.UpdateSongs(updateNumber, JsonToDictionary(await FetchDataAsync(updateJsonURL)), false);
                    }
                }
            }

            return 0;
        }

        public static Dictionary<string, string> JsonToDictionary(JObject json)
        {
            Dictionary<string, string> updateDict = new Dictionary<string, string>();

            foreach (var updateEntry in json)
            {
                updateDict.Add(updateEntry.Key, updateEntry.Value.ToObject<string>());
            }

            return updateDict;
        }
    }
}