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
using Xamarin.Essentials;
using System.Security.Cryptography;


namespace Project12
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        EditText etSignUpUsername, etPhoneNumber, etSignUpPassword;
        Button btnSignUpConfirm, btnBackButton;
        FirebaseManager firebase;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.sign_up);
            firebase = new FirebaseManager("https://project12-f950c-default-rtdb.europe-west1.firebasedatabase.app/");

            etSignUpUsername = FindViewById<EditText>(Resource.Id.etSignUpUsername);
            etSignUpPassword = FindViewById<EditText>(Resource.Id.etSignUpPassword);
            etPhoneNumber = FindViewById<EditText>(Resource.Id.etSignUpPhonrNumber);
            btnSignUpConfirm = FindViewById<Button>(Resource.Id.btnSignUpConfirm);
            btnBackButton = FindViewById<Button>(Resource.Id.btnBackButton);

            btnSignUpConfirm.Click += BtnSignUpConfirm_Click;
            btnBackButton.Click += BtnBackButton_Click;
        }

        private void BtnBackButton_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private async void BtnSignUpConfirm_Click(object sender, System.EventArgs e)
        {
            string username = etSignUpUsername.Text;
            string phoneNumber = etPhoneNumber.Text;
            string password = etSignUpPassword.Text;

            // Add sign-up logic here (e.g., save user data, validation)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                Toast.MakeText(this, "Please fill all fields", ToastLength.Short).Show();
            }
            else
            {

                try
                {
                    await firebase.GetAccountAsync(phoneNumber);
                    new AlertDialog.Builder(this)
                        .SetMessage("Invalid phon number").SetPositiveButton("OK", (sender, args) => { }).Show();

                }

                catch
                {
                    {


                        await firebase.SaveAccountAsync(new Account(phoneNumber, username, password));

                        new AlertDialog.Builder(this)
                            .SetMessage("Signed up seccessfully").SetPositiveButton("OK", (sender, args) => { }).Show();

                        // After successful sign-up, you can navigate back to the login screen
                        Finish(); // This will close the current activity and go back to the previous one (Sign In)
                    }
                }
            }
        }
    }

}