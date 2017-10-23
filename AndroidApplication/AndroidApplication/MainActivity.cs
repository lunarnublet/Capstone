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
using System.Threading.Tasks;
using Java.Nio;
using Android.Runtime;
using System.Text;
using System.IO.Compression;

namespace AndroidApplication
{
     [Activity(Label = "AndroidApplication", MainLauncher = true)]
     public class MainActivity : Activity
     {
          public static readonly int PickImageId = 1000;
          private List<Android.Net.Uri> imageUris = new List<Android.Net.Uri>();
          private Server server;
          private ImageView ImageView;

          protected override void OnCreate(Bundle savedInstanceState)
          {
               base.OnCreate(savedInstanceState);

               // Set our view from the "main" layout resource
               SetContentView(Resource.Layout.Main);
               Button selectPhoto = FindViewById<Button>(Resource.Id.SelectPhotoButton);
               Button addConnection = FindViewById<Button>(Resource.Id.AddConnectionButton);
               ImageView = FindViewById<ImageView>(Resource.Id.MyImage);
               selectPhoto.Click += OnSelectPhotoButtonClick;
               addConnection.Click += OnAddConnectionButtonClick;

               server = new Server(this);
               server.OnStart();
          }
          private void OnSelectPhotoButtonClick(object sender, EventArgs eventArgs)
          {
               Intent intent = new Intent();
               intent.SetType("image/*");
               intent.PutExtra(Intent.ExtraAllowMultiple, true);
               intent.SetAction(Intent.ActionGetContent);
               StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
          }

          private void OnAddConnectionButtonClick(object sender, EventArgs eventArgs)
          {
               //Intent intent = new Intent();
               //intent.SetType("image/*");
               //intent.PutExtra(Intent.ExtraAllowMultiple, true);
               //intent.SetAction(Intent.ActionGetContent);
               //StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
               string s = FindViewById<EditText>(Resource.Id.MyEditText).Text;
               FindViewById<TextView>(Resource.Id.MyTextView).Text = s;

               server.AddConnection("e60M6GbuxK");
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
                    FindViewById<TextView>(Resource.Id.MyTextView).Text = "Converting Uris: " + imageUris.Count.ToString();
                    ConvertUris();
               }
          }

          //private void AddCellsToListView(List<Android.Net.Uri> imageUris)
          //{
          //     foreach (var item in imageUris) 
          //     {

          //     }
          //}

          //public void ConvertUris()
          //{
          //     foreach (var uri in imageUris)
          //     {
          //          string s = ToBase64String(uri);
          //          server.SendPhoto(s);
          //     }
          //}

          public void ConvertUris()
          {
               foreach (var uri in imageUris)
               {
                    string s = ToBase64String(uri);
                    DisplayImage(s);
                    server.SendPhoto(s);
               }
          }

          private void UnzipAndDisplayImage(byte[] bytes)
          {
               string base64 = Unzip(bytes);
               DisplayImage(base64);
          }

          private void DisplayImage(string image) 
          {
               byte[] imagebytes = Convert.FromBase64String(image);
               Bitmap bitmap = BitmapFactory.DecodeByteArray(imagebytes, 0, imagebytes.Length);
               ImageView.SetImageBitmap(bitmap);
          }

          public string ToBase64String(Android.Net.Uri uri)
          {
               string output = "";
               Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uri);
               //ByteBuffer buffer = ByteBuffer.Allocate(bitmap.ByteCount);
               //bitmap.CopyPixelsToBuffer(buffer);
               //buffer.Rewind();

               //IntPtr classHandle = JNIEnv.FindClass("java/nio/ByteBuffer");
               //IntPtr methodId = JNIEnv.GetMethodID(classHandle, "array", "()[B");
               //IntPtr resultHandle = JNIEnv.CallObjectMethod(buffer.Handle, methodId);
               //byte[] bytes = JNIEnv.GetArray<byte>(resultHandle);
               //JNIEnv.DeleteLocalRef(resultHandle);

               //output = Convert.ToBase64String(bytes);
               //int v = output.Length;

               using (var stream = new MemoryStream())
               {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

                    var bytes = stream.ToArray();

                    var str = Convert.ToBase64String(bytes);
                    output = str;
               }
               return output;
          }

          public byte[] ToZippedByteArray(Android.Net.Uri uri)
          {
               string s = ToBase64String(uri);
               byte[] output = Zip(s);
               return output;
          }

          public static void CopyTo(Stream src, Stream dest)
          {
               byte[] bytes = new byte[4096];

               int cnt;

               while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
               {
                    dest.Write(bytes, 0, cnt);
               }
          }

          public static byte[] Zip(string str)
          {
               var bytes = Encoding.UTF8.GetBytes(str);

               using (var msi = new MemoryStream(bytes))
               using (var mso = new MemoryStream())
               {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                         //msi.CopyTo(gs);
                         CopyTo(msi, gs);
                    }

                    return mso.ToArray();
               }
          }

          public static string Unzip(byte[] bytes)
          {
               using (var msi = new MemoryStream(bytes))
               using (var mso = new MemoryStream())
               {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                         //gs.CopyTo(mso);
                         CopyTo(gs, mso);
                    }

                    return Encoding.UTF8.GetString(mso.ToArray());
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

