using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Phoneword;
using System.Collections.Generic;

namespace Phoneword
{
    [Activity(Label = "Phone word", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        static readonly List<string> phoneNumbers = new List<string>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
            Button translateButton = FindViewById<Button>(Resource.Id.TranslateButton);
            Button callButton = FindViewById<Button>(Resource.Id.CallButton);
            Button callHistoryButton = FindViewById<Button>(Resource.Id.CallHistoryButton);

            callButton.Enabled = false;

            string translatedNumber = string.Empty;

            //when user presses translate button translate string to number and enable calling function.
            translateButton.Click += (object sender, EventArgs e) =>
            { 
                translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
                if (String.IsNullOrWhiteSpace(translatedNumber))
                {
                    callButton.Text = "Call";
                    callButton.Enabled = false;
                }
                else
                {
                    callButton.Text = "Call " + translatedNumber;
                    callButton.Enabled = true;
                }
            };



            callButton.Click += (object sender, EventArgs e) =>
            {
                //create alert dialog that asks if user really wants to call
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Call " + translatedNumber + "?");
                callDialog.SetNeutralButton("Call", delegate{

                    //add phone number to list
                    phoneNumbers.Add(translatedNumber);
                    callHistoryButton.Enabled = true;

                    //call the number
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel: " + translatedNumber));
                    StartActivity(callIntent);
                });

                callDialog.SetNegativeButton("Cancel", delegate { });
                callDialog.Show();
            };

            callHistoryButton.Click += (object sender,EventArgs e) =>
            {
                var intent = new Intent(this, typeof(CallHistoryActivity));
                intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };
        }
    }
}

