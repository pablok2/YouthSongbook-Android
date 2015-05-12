#region

using Android.App;
using Android.OS;
using Java.Lang;

#endregion

namespace YouthSongbook
{
    [Activity(MainLauncher = true, Theme = "@style/Theme.Splash")]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Thread.Sleep(500);
            StartActivity(typeof (MainActivity));
            Finish();
        }

        protected override void OnResume()
        {
            Finish();
        }
    }
}