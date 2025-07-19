using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project12
{
    [Activity(Label = "RecentTransfers")]
    public class RecentTransfers : Activity
    {
        private ListView listViewTransfers;      // ListView to display transfer items
        private Button btnBackButton;   

        private FirebaseManager firebaseManager;  // Custom manager to interact with Firebase Realtime Database
        private Account account;                  // Current user's account instance
        private ISharedPreferences sharedPreferences;  // SharedPreferences are used for persistent key-value storage between sessions
        private string accountId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.recent_transfers);

            firebaseManager = new FirebaseManager("https://project12-f950c-default-rtdb.europe-west1.firebasedatabase.app/");

            // Instead of Intent extras, retrieve the account ID from SharedPreferences

            sharedPreferences = this.GetSharedPreferences("details", FileCreationMode.Private);
            accountId = sharedPreferences.GetString("id", null);
            listViewTransfers = FindViewById<ListView>(Resource.Id.listViewTransfers);
            btnBackButton = FindViewById<Button>(Resource.Id.btnBackButton);
            btnBackButton.Click += BtnBackButton_Click;
            // Create your application here
        }

        protected override async void OnResume()
        {
            base.OnResume();
            try
            {
                account = await firebaseManager.GetAccountAsync(accountId);


                if (account.Transfers != null)
                {

                    listViewTransfers.Adapter = new TransferAdapter(this, account.Transfers, accountId,
                        "https://project12-f950c-default-rtdb.europe-west1.firebasedatabase.app/", true);

                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error: " + ex.Message, ToastLength.Long).Show();
            }


        }

        private void BtnBackButton_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}