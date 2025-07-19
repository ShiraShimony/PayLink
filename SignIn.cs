using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using Xamarin.Essentials;

namespace Project12
{
    [Activity(Label = "openScreen")]
    public class SignIn : Activity
    {
        EditText etPhoneNumber;
        EditText etPassword;
        Button btnSignIn, btnSignUp;
        FirebaseManager firebase; 
        ISharedPreferences sharedPreferences;

        async protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sign_in);

            etPhoneNumber = FindViewById<EditText>(Resource.Id.etPhoneNumber);
            etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            btnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            firebase = new FirebaseManager("https://project12-f950c-default-rtdb.europe-west1.firebasedatabase.app/");



            sharedPreferences = this.GetSharedPreferences("details", FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.Clear();

            btnSignIn.Click += BtnSignIn_Click;

            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            btnSignUp.Click += BtnSignUp_Click;
        }

        async private void BtnSignIn_Click(object sender, System.EventArgs e)
        {
            string phoneNumber = etPhoneNumber.Text;
            string password = etPassword.Text;

            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))
            {
                Toast.MakeText(this, "Please enter both phoneNumber and password", ToastLength.Short).Show();
                return;
            }


            try
            {
                Account account = await firebase.GetAccountAsync(phoneNumber);
                bool valid_pass = await firebase.CheckPassword(phoneNumber, password);

                if (valid_pass)
                {
                    ISharedPreferencesEditor editor = sharedPreferences.Edit();
                    editor.PutString("id", phoneNumber);
                    editor.Commit();


                    Finish();
                }

                else
                {
                    Toast.MakeText(this, "Incurrect password", ToastLength.Short);
                }
            }
            catch
            {   
                Toast.MakeText(this, "phoneNumber does not exist", ToastLength.Short).Show();
            }
        }

        private void BtnSignUp_Click(object sender, System.EventArgs e)
        {
            // Navigate to Sign Up activity
            var intent = new Intent(this, typeof(SignUpActivity));
            StartActivity(intent);
        }
    }
}