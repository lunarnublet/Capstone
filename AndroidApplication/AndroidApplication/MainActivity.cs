using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Net;
using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Provider;
using System.Drawing;
using System.IO;
using Android.Util;

namespace AndroidApplication
{
     [Activity(Label = "AndroidApplication", MainLauncher = true)]
     public class MainActivity : Activity
     {
          public static readonly int PickImageId = 1000;
          private ListView listview;
          private List<Android.Net.Uri> imageUris = new List<Android.Net.Uri>();
          private Server server;

          protected override void OnCreate(Bundle savedInstanceState)
          {
               base.OnCreate(savedInstanceState);

               // Set our view from the "main" layout resource
               SetContentView(Resource.Layout.Main);
               listview = FindViewById<ListView>(Resource.Id.MyListView);
               Button button = FindViewById<Button>(Resource.Id.MyButton);
               button.Click += ButtonOnClick;

               server = new Server(this);
               server.OnStart();
          }
          private void ButtonOnClick(object sender, EventArgs eventArgs)
          {
               Intent intent = new Intent();
               intent.SetType("image/*");
               intent.PutExtra(Intent.ExtraAllowMultiple, true);
               intent.SetAction(Intent.ActionGetContent);
               StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
          }

          protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
          {
               if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
               {

                    if (data.ClipData == null)
                    {
                         imageUris = new List<Android.Net.Uri>();
                         imageUris.Add(data.Data);
                    }
                    else 
                    {
                         imageUris = new List<Android.Net.Uri>();
                         var list = ToList(data.ClipData);
                         foreach (var item in list)
                         {
                              imageUris.Add(item);
                         }
                    }
                    FindViewById<TextView>(Resource.Id.MyTextView).Text = "            " + imageUris.Count.ToString();
                    //AddCellsToListView(imageUris);
               }
          }

          //private void AddCellsToListView(List<Android.Net.Uri> imageUris)
          //{
          //     foreach (var item in imageUris) 
          //     {
                   
          //     }
          //}

          public string ToBase64String(Android.Net.Uri uri) 
          {
               string output = "";
               Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uri);
               using (var stream = new MemoryStream())
               {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);

                    var bytes = stream.ToArray();
                    var str = Convert.ToBase64String(bytes);
                    output = str;
               }
               return output;
          }

          public static List<Android.Net.Uri> ToList(ClipData clipData)
          {
               var list = new List<Android.Net.Uri>();
               for (int i = 0; i < clipData.ItemCount; ++i)
               {
                    list.Add(clipData.GetItemAt(i).Uri);
               }
               return list;
          }

     }
}

