#region

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using HIHSongbook;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "Hand in Hand Songbook", Icon = "@drawable/icon",
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
            highContrastEnabled = SongData.GetSetting(Setting.Contrast);
            if (highContrastEnabled)
            {
                SetContentView(Resource.Layout.MainHC);
            }
            else
            {
                SetContentView(Resource.Layout.Main);
            }

            listView = FindViewById<ListView>(Resource.Id.list);
            listView.FastScrollEnabled = true;
            listView.ScrollBarStyle = ScrollbarStyles.OutsideInset;

            // Send song title to the song displaying class
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Intent intent = new Intent(this, typeof(SongActivity));
                intent.PutExtra("SONG_NAME", songNames[e.Position]);
                intent.PutExtra("CHORDS", chordsEnabled);
                StartActivity(intent);
                OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            };
        }

        protected override void OnResume()
        {
            // Reload all the initial stuff
            // for HC layout changes
            OnCreate(thisBundle);

            base.OnResume();

            // Get the chords flag
            chordsEnabled = SongData.GetSetting(Setting.Chords);

            // Reload
            songNames = SongData.GetAllTitles(chordsEnabled);

            // Check for updates
            if (SongData.GetSetting(Setting.UpdateFlag))
            {
                UpdateAsync();
            }

            listView.Adapter = new SongListAdapter(this, songNames, highContrastEnabled);
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
                    StartActivity(typeof (UpdateActivity));
                    OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
                    return true;
                }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        // Async update method
        private async Task UpdateAsync()
        {
            try
            {
                await SongNetwork.PerformUpdateAsync();
            }
            catch (Exception)
            {
                // ignore attempt
            }
        }
    }
}