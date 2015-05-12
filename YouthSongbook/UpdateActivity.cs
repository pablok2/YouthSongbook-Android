#region

using Android.App;
using Android.OS;
using Android.Widget;
using HIHSongbook;

#endregion

namespace YouthSongbook
{
    [Activity(Label = "Settings", Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar")]
    public class UpdateActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // Hook up the view
            base.OnCreate(bundle);
            if (SongData.GetSetting(Setting.Contrast))
            {
                SetContentView(Resource.Layout.UpdateLayoutHC);
            }
            else
            {
                SetContentView(Resource.Layout.UpdateLayout);
            }

            Switch updateSwitch = FindViewById<Switch>(Resource.Id.autoUpdateSwitch);
            Switch chordSwitch = FindViewById<Switch>(Resource.Id.chordSwitch);
            Switch hcSwitch = FindViewById<Switch>(Resource.Id.hcSwitch);

            chordSwitch.Checked = SongData.GetSetting(Setting.Chords);
            hcSwitch.Checked = SongData.GetSetting(Setting.Contrast);
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