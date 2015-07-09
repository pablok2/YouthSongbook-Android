#region

using System.Threading;
using Android;
using Android.App;
using Android.OS;

#endregion

namespace YouthSongbook
{
    [Activity(MainLauncher = true, Theme = "@style/Theme.Splash")]
    public class SplashActivity : Activity
    {
        /// <summary>
        /// Starter activity
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof (MainActivity));
            Thread.Sleep(500);
            OverridePendingTransition(Resource.Animation.FadeIn, Resource.Animation.FadeOut);

            Finish();
        }
    }
}