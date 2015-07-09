#region

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using HIHSongbook;
using Android.Graphics;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "Hand in Hand Songbook", Icon = "@drawable/icon_transparent_blue",
        Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class MainActivity : Activity
    {
        private ListView listView;
        private string[] songNames;
        private Bundle thisBundle;
        private int selectedItem = 0;

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
            SetContentView(Resource.Layout.Main);
            
            listView = FindViewById<ListView>(Resource.Id.list);
            listView.FastScrollEnabled = true;
            listView.ScrollBarStyle = ScrollbarStyles.OutsideInset;

            // Send song title to the song displaying class
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Intent intent = new Intent(this, typeof (SongActivity));
                intent.PutExtra("SONG_NAME", songNames[e.Position]);
                StartActivity(intent);
                OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
                selectedItem = e.Position;
            };
        }

        protected override void OnResume()
        {
            // Check for updates
            if (SongData.GetSetting(Setting.UpdateFlag))
            {
                UpdateAsync();
            }  

            base.OnResume();

            // Get flags
            bool chordsEnabled = SongData.GetSetting(Setting.Chords);
            bool highContrastEnabled = SongData.GetSetting(Setting.Contrast);

            songNames = SongData.GetAllTitles(chordsEnabled);

            // Reload the adapter
            listView.Adapter = new SongListAdapter(this, songNames, SongData.GetSetting(Setting.Contrast));
            listView.SetBackgroundColor(highContrastEnabled ? Color.Black : Color.White);
            listView.SetSelection(selectedItem);
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