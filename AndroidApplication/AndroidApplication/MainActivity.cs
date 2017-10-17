using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Provider;
using System.Drawing;
using System.IO;

namespace AndroidApplication
{
     [Activity(Label = "AndroidApplication", MainLauncher = true)]
     public class MainActivity : Activity
     {
          public static readonly int PickImageId = 1000;
          private ImageView _imageView;
          protected override void OnCreate(Bundle savedInstanceState)
          {
               base.OnCreate(savedInstanceState);

               // Set our view from the "main" layout resource
               SetContentView(Resource.Layout.Main);
               _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
               Button button = FindViewById<Button>(Resource.Id.MyButton);
               button.Click += ButtonOnClick;
          }
          private void ButtonOnClick(object sender, EventArgs eventArgs)
          {

               //var imageIntent = new Intent(Intent.ActionPick);
               //imageIntent.SetType("image/*");
               //imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);
               //imageIntent.SetAction(Intent.ActionGetContent);
               //StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), PickImageId);
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
                    TextView textView = FindViewById<TextView>(Resource.Id.MyTextView);
                    string s = "";

                    if (data.ClipData == null)
                    {

                         Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data);
                         using (var stream = new MemoryStream())
                         {
                              bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);

                              var bytes = stream.ToArray();
                              var str = Convert.ToBase64String(bytes);
                              s += str;
                         }
                    }
                    else 
                    {
                         var list = ToList(data.ClipData);
                         foreach (var item in list)
                         {
                              Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, item);
                              using (var stream = new MemoryStream())
                              {
                                   bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);

                                   var bytes = stream.ToArray();
                                   var str = Convert.ToBase64String(bytes);
                                   s += str;
                              }
                         }
                    }
                    textView.Text = s;
               }
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

