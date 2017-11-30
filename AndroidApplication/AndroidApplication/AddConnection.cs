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
using Android.Content.PM;

namespace AndroidApplication
{
     [Activity(Label = "Easy Upload", MainLauncher = false, Theme = "@android:style/Theme.Holo.Dialog.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
     class AddConnection : Activity
     {
          Button addButton;
          EditText codeEdit;

          protected override void OnCreate(Bundle savedInstanceState)
          {
               base.OnCreate(savedInstanceState);

               SetContentView(Resource.Layout.AddConnection);
               addButton = FindViewById<Button>(Resource.Id.AddButton);

               codeEdit = FindViewById<EditText>(Resource.Id.CodeEditText);
               addButton.Click += AddButton_Click;
          }

          private void AddButton_Click(object sender, EventArgs e)
          {
               Intent myIntent = new Intent(this, typeof(MainActivity));
               myIntent.PutExtra("code", codeEdit.Text);
               SetResult(Result.Ok, myIntent);
               Finish();
          }
     }
}