#region

using Android;
using Android.App;
using Android.OS;

#endregion

namespace YouthSongbook
{
    [Activity(MainLauncher = true, Theme = "@style/Theme.Splash")]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof (MainActivity));
            Finish();
            OverridePendingTransition(Resource.Animation.FadeIn, Resource.Animation.FadeOut);
        }
    }
}