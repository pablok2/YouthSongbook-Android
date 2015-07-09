#region

using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Android.Content;
using HIHSongbook;
using Android.Net;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "Settings", Icon = "@drawable/icon_transparent", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class UpdateActivity : Activity
    {
        /// <summary>
        /// Called when the screen gets loaded
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            // Hook up the view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UpdateLayout);            
            
            bool highContrast = SongData.GetSetting(Setting.Contrast);

            // Setup the colors of text and the view
            RelativeLayout updateLayout = FindViewById<RelativeLayout>(Resource.Id.relativeLayout1);
            updateLayout.SetBackgroundColor(highContrast ? Color.Black : Color.White);

            // Get the switch settings
            Switch updateSwitch = FindViewById<Switch>(Resource.Id.autoUpdateSwitch);
            Switch chordSwitch = FindViewById<Switch>(Resource.Id.chordSwitch);
            Switch hcSwitch = FindViewById<Switch>(Resource.Id.hcSwitch);
           
            chordSwitch.Checked = SongData.GetSetting(Setting.Chords);
            hcSwitch.Checked = highContrast;
            updateSwitch.Checked = SongData.GetSetting(Setting.UpdateFlag);

            // Switch label colors
            Color textColor = highContrast ? Color.White : Color.Black;
            chordSwitch.SetTextColor(textColor);
            hcSwitch.SetTextColor(textColor);
            updateSwitch.SetTextColor(textColor);

            TextView weblink = FindViewById<TextView>(Resource.Id.webLink);            
            
            // Auto Update Switch
            updateSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                SongData.SetSetting(Setting.UpdateFlag, e.IsChecked);
            };

            // Chord switch
            chordSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                SongData.SetSetting(Setting.Chords, e.IsChecked);
            };

            // High Contrast switch
            hcSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                SongData.SetSetting(Setting.Contrast, e.IsChecked);
            };


            weblink.Click += delegate(object sender, System.EventArgs e)
            {
                Intent browserIntent = new Intent(Intent.ActionView, Uri.Parse("http://www.handforyou.org"));
                StartActivity(browserIntent);
            };
        }
    }
}