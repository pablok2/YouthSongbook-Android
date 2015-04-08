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

namespace YouthSongbook
{
    [Activity(Label = "Settings", Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light")]
    public class UpdateActivity : Activity
    {
        Button updateButton;
        CheckBox chordsCheckBox;

        protected override void OnCreate(Bundle bundle)
        {   
            // Hook up the view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UpdateLayout);
            updateButton = FindViewById<Button>(Resource.Id.updateButton);
            chordsCheckBox = FindViewById<CheckBox>(Resource.Id.chordsCheckBox);

            // Set the chordsCheckBox if the current database is chords
            bool chordsActive = SongData.GetChords();
            chordsCheckBox.Checked = chordsActive;

            // Async update button
            updateButton.Click += async (o, e) =>
                    {
                        try
                        {
                            await SongNetwork.PerformUpdateAsync();
                            Toast.MakeText(this, "Finished Updating", ToastLength.Short).Show();
                        }
                        catch(Exception)
                        {
                            Toast.MakeText(this, "Unable to Update", ToastLength.Short).Show();
                        }
                    };

            // Setting the chords database
            chordsCheckBox.Click += (o, e) =>
            {
                if (chordsCheckBox.Checked)
                {
                    SongData.SetChords(true);
                    Toast.MakeText(this, "Enabled Chords", ToastLength.Short).Show();
                }
                else
                {
                    SongData.SetChords(false);
                    Toast.MakeText(this, "Disabled Chords", ToastLength.Short).Show();
                }
            };
        }
    }
}