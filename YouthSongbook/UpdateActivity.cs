#region

using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using HIHSongbook;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "Settings", Icon = "@drawable/icon_transparent", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class UpdateActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // Hook up the view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UpdateLayout);

            Switch updateSwitch = FindViewById<Switch>(Resource.Id.autoUpdateSwitch);
            Switch chordSwitch = FindViewById<Switch>(Resource.Id.chordSwitch);
            Switch hcSwitch = FindViewById<Switch>(Resource.Id.hcSwitch);

            bool highContrast = SongData.GetSetting(Setting.Contrast);

            // Setup the colors of text and the view
            RelativeLayout updateLayout = FindViewById<RelativeLayout>(Resource.Id.relativeLayout1);
            updateLayout.SetBackgroundColor(highContrast ? Color.Black : Color.White);

            Color textColor = highContrast ? Color.White : Color.Black;
            chordSwitch.SetTextColor(textColor);
            hcSwitch.SetTextColor(textColor);
            updateSwitch.SetTextColor(textColor);

            // Get the switch settings
            chordSwitch.Checked = SongData.GetSetting(Setting.Chords);
            hcSwitch.Checked = highContrast;
            updateSwitch.Checked = SongData.GetSetting(Setting.UpdateFlag);

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
        }
    }
}