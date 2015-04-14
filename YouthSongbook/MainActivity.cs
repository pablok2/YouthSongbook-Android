using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;

namespace YouthSongbook
{
    [Activity(Label = "New Youth Songbook", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class MainActivity : Activity
    {
        ListView listView;
        string[] songNames;
        bool chordsEnabled;
        bool highContrastEnabled;

        protected override void OnCreate(Bundle bundle)
        {
            // Check for a living database and create if need be
            if(!SongData.DataBaseExists)
            {
                SongData.LoadDatabase(Assets.Open("songs.json"), false);
                SongData.LoadDatabase(Assets.Open("songsChords.json"), true);
            }
            
            // Hook up the views
            base.OnCreate(bundle);
            //highContrastEnabled = SongData.GetContrast();
            //if (highContrastEnabled)
            //{
            //    listView.SetBackgroundColor(Color.Black);
            //}

            SetContentView(Resource.Layout.Main);
            listView = FindViewById<ListView>(Resource.Id.list);

            // Send song title to the song displaying class
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Intent intent = new Intent(this, typeof(SongActivity));
                intent.PutExtra("SONG_NAME", songNames[e.Position]);
                intent.PutExtra("CHORDS", chordsEnabled);
                this.StartActivity(intent);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Get the chords flag
            chordsEnabled = SongData.GetChords();
            //highContrastEnabled = SongData.GetContrast();

            // Reload
            songNames = SongData.GetAllTitles(chordsEnabled);
            listView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, songNames);
        }

        // Menu item(s)
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, "Settings");
            return true;
        }

        // What happens when an item in the menu is selected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    {
                        // Send clean intent to the update class
                        this.StartActivity(new Intent(this, typeof(UpdateActivity)));
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}

