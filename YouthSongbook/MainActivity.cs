﻿#region

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "New Youth Songbook", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class MainActivity : Activity
    {
        private bool chordsEnabled;
        private bool highContrastEnabled;
        private ListView listView;
        private string[] songNames;
        private Bundle thisBundle;

        protected override void OnCreate(Bundle bundle)
        {
            // Hook up the views
            base.OnCreate(bundle);
            thisBundle = bundle;

            // Check for a living database and create if need be
            if (!SongData.DataBaseExists)
            {
                SongData.LoadDatabase(Assets.Open("songs.json"), false);
                SongData.LoadDatabase(Assets.Open("songsChords.json"), true);
            }

            // Load contrast settings
            highContrastEnabled = SongData.GetContrast();
            if (highContrastEnabled)
            {
                SetContentView(Resource.Layout.MainHC);
            }
            else
            {
                SetContentView(Resource.Layout.Main);
            }

            listView = FindViewById<ListView>(Resource.Id.list);

            // Send song title to the song displaying class
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Intent intent = new Intent(this, typeof (SongActivity));
                intent.PutExtra("SONG_NAME", songNames[e.Position]);
                intent.PutExtra("CHORDS", chordsEnabled);
                StartActivity(intent);
            };
        }

        protected override void OnResume()
        {
            // Reload all the initial stuff
            // for HC layout changes
            OnCreate(thisBundle);

            base.OnResume();

            // Get the chords flag
            chordsEnabled = SongData.GetChords();

            // Reload
            songNames = SongData.GetAllTitles(chordsEnabled);

            // Set list adapter type
            if (highContrastEnabled)
            {
                listView.Adapter = new ArrayAdapter<string>(this, Resource.Layout.simple_list_item_CUST, songNames);
            }
            else
            {
                listView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, songNames);
            }
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
                    StartActivity(new Intent(this, typeof (UpdateActivity)));
                    return true;
                }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}