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
        Button button;

        protected override void OnCreate(Bundle bundle)
        {   
            // Hook up the view
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UpdateLayout);
            button = FindViewById<Button>(Resource.Id.updateButton);

            // Async update button
            button.Click += async (sender, e) =>
                    {
                        await SongNetwork.PerformUpdateAsync();
                        Finish();
                    };
            
            // Need to change this class to have a progress
            // or spinny thingy while it's updating.
        }
    }
}