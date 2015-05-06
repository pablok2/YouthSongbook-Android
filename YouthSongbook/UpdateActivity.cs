#region

using Android.App;
using Android.OS;
using Android.Widget;

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
            if (SongData.GetContrast())
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

            chordSwitch.Checked = SongData.GetChords();
            hcSwitch.Checked = SongData.GetContrast();
            updateSwitch.Checked = SongData.GetUpdateFlag();

            // Auto Update Switch
            updateSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                SongData.SetUpdateFlag(e.IsChecked);
            };

            // Chord switch
            chordSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                SongData.SetChords(e.IsChecked);
            };

            // High Contrast switch
            hcSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                SongData.SetContrast(e.IsChecked);
            };
        }
    }
}