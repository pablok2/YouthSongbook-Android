using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace YouthSongbook
{
    [Activity(Label = "SongActivity", Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class SongActivity : Activity
    {
        TextView songText;

        protected override void OnCreate(Bundle bundle)
        {
            // Hook up the views
            base.OnCreate(bundle);
            if (SongData.GetContrast())
            {
                SetContentView(Resource.Layout.SongLayoutHC);
            }
            else
            {
                SetContentView(Resource.Layout.SongLayout);
            }

            songText = FindViewById<TextView>(Resource.Id.songText);

            // Get the song title and chords from the intent and set the title
            string songName = Intent.GetStringExtra("SONG_NAME");
            bool chords = Intent.GetBooleanExtra("CHORDS", false);

            Title = songName;

            // Populate the test view with the song
            songText.SetText(SongData.GetSong(songName, chords), TextView.BufferType.Normal);
        }
    }
}