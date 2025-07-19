    using Android.App;
    using Android.OS;
    using Android.Widget;
    using Android.Content;
    using Android.Preferences;
    using System;
    using System.Collections.Generic;
    using System.Linq;
using Android.Accounts;
using AndroidX.AppCompat.App;
using Android.Icu.Text;

namespace Project12
{
    /// <summary>
    /// Activity that displays account details including balance and recent transfers.
    /// Provides UI for initiating transfers, requests, and viewing transfer history.
    /// </summary>
    [Activity(Label = "MainPage")]
    public class MainPage : AppCompatActivity
    {
        private TextView textViewName;      // TextView to display user name
        private TextView textViewReminder;      // TextView to display account balance
        private Button buttonTransfer;          // Button to initiate transfer
        private Button buttonRequest;          // Button to initiate request
        private ListView listViewTransfers;      // ListView to display transfer items

        private FirebaseManager firebaseManager;  // Custom manager to interact with Firebase Realtime Database
        private Account account;                  // Current user's account instance
        private ISharedPreferences sharedPreferences;  // SharedPreferences are used for persistent key-value storage between sessions
        private string accountId;
        private Android.App.AlertDialog dialog;



        /// <summary>
        /// Called when the activity is created. Initializes UI and loads account data.
        /// </summary>
        /// <param name="savedInstanceState">If the activity is being re-initialized after previously being shut down then this Bundle contains the data it most recently supplied. Otherwise, it is null.</param>
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.main_page);

            // Linking layout components to fields
            textViewName = FindViewById<TextView>(Resource.Id.textViewName);
            textViewReminder = FindViewById<TextView>(Resource.Id.textViewReminder);
            buttonTransfer = FindViewById<Button>(Resource.Id.buttonTransfer);
            buttonRequest = FindViewById<Button>(Resource.Id.buttonRequest);
            listViewTransfers = FindViewById<ListView>(Resource.Id.listViewTransfers);


            var intent = new Intent(this, typeof(SignIn));
            StartActivity(intent);
            //var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            //SetActionBar(toolbar);



            



            

            buttonTransfer.Click += delegate
            {
                var intent = new Intent(this, typeof(MakeTransfer));
                intent.PutExtra("isRequestMode", false);
                StartActivity(intent);
            };

            buttonRequest.Click += delegate
            {
                var intent = new Intent(this, typeof(MakeTransfer));
                intent.PutExtra("isRequestMode", true);
                StartActivity(intent);
            };






        }
        private async void UpdateBalance()
        {
            Account acc = await firebaseManager.GetAccountAsync(account.Id);
            textViewReminder.Text = "Balance: " + acc.Remainder + "₪";
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
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("");
                builder.SetMessage("Are you sure you want to logout?");
                builder.SetCancelable(true);
                builder.SetPositiveButton("yes", LogOut);
                builder.SetNegativeButton("No", Cancel);
                dialog = builder.Create();
                dialog.Show();
                return true;

            }
            else if (item.ItemId == Resource.Id.action_home)
            {
                Toast.MakeText(this, "you are in page home", ToastLength.Long).Show();
                return true;
            }
            else
            {
                ISharedPreferencesEditor editor = sharedPreferences.Edit();
                editor.PutString("id", accountId);
                editor.Commit();
                var intent = new Intent(this, typeof(RecentTransfers));
                StartActivity(intent);
                return true;
            }
            return base.OnOptionsItemSelected(item);

        }

        private void LogOut(object sender, DialogClickEventArgs e)
        {
            // Clear shared preferences
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.Clear();
            editor.Commit();

            Toast.MakeText(this, "Logged out successfully", ToastLength.Short).Show();
            Finish();
        }

        private void Cancel(object sender, DialogClickEventArgs e)
        {

        }


        protected override async void OnResume()
        {
            base.OnResume();
            firebaseManager = new FirebaseManager("https://project12-f950c-default-rtdb.europe-west1.firebasedatabase.app/");

            // Instead of Intent extras, retrieve the account ID from SharedPreferences

            sharedPreferences = this.GetSharedPreferences("details", FileCreationMode.Private);
            accountId = sharedPreferences.GetString("id", null);

            if (accountId == null)
            {
                new Android.App.AlertDialog.Builder(this)
                    .SetTitle("Error")
                    .SetMessage("Account ID not found. Please log in again.")
                    .SetPositiveButton("OK", (sender, args) =>
                    {
                        Intent intent = new Intent(this, typeof(SignIn));
                        StartActivity(intent);
                        Finish(); // close current activity
                    })
                    .SetCancelable(false)
                    .Show();

                return;
            }

            try
            {
                account = await firebaseManager.GetAccountAsync(accountId);

                textViewName.Text = "Hello " + account.Name;
                textViewReminder.Text = "Balance: " + account.Remainder + "₪";

                if (account.Transfers != null)
                {

                    listViewTransfers.Adapter = new TransferAdapter(this, account.GetWaitingTransfers(), accountId,
                        "https://project12-f950c-default-rtdb.europe-west1.firebasedatabase.app/",false, UpdateBalance);

                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message, ToastLength.Long).Show();
            }


        }



    }
}


