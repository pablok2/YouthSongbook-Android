#region

using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using HIHSongbook;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "SongActivity", Icon = "@drawable/icon_transparent_blue", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class SongActivity : Activity
    {
        /// <summary>
        /// Called when the screen gets loaded
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            // Hook up the views
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.SongLayout);

            TextView songText = FindViewById<TextView>(Resource.Id.songText);
            LinearLayout songLayout = FindViewById<LinearLayout>(Resource.Id.songLayout);

            // Get the song title and chords from the intent and set the title
            string songName = Intent.GetStringExtra("SONG_NAME");
            bool chords = SongData.GetSetting(Setting.Chords);
            bool highContrast = SongData.GetSetting(Setting.Contrast);        
            
            // Populate the text view with the song
            this.Title = songName;            
            songText.SetText(SongData.GetSong(songName, chords), TextView.BufferType.Normal);

            // Set the color schema
            songLayout.SetBackgroundColor(highContrast ? Color.Black : Color.White);
            songText.SetBackgroundColor(highContrast ? Color.Black : Color.White);
            songText.SetTextColor(highContrast ? Color.White : Color.Black);
        }
    }
}