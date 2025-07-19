using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace Project12
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            var intent = new Intent(this, typeof(MainPage));
            StartActivity(intent);
            //SupportActionBar.Hide();
            
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.a_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_logout)
            {
                Toast.MakeText(this, "you selected logout", ToastLength.Long).Show();
                return true;

            }
            else if (item.ItemId == Resource.Id.action_home)
            {
                Toast.MakeText(this, "you selected home", ToastLength.Long).Show();
                return true;
            }
            else
            {
                Toast.MakeText(this, "you selected Recent Transfers", ToastLength.Long).Show();
                return true;
            }
            return base.OnOptionsItemSelected(item);

        }
        protected override async void OnResume()
        {
            base.OnResume();
            Finish();
        }
    }

}