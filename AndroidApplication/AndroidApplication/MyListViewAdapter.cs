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
using Android.Graphics;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Database;
using Android.Provider;

namespace AndroidApplication
{
     class MyListViewAdapter : BaseAdapter<Tuple<Android.Net.Uri, string>>
     {
          private List<Tuple<Android.Net.Uri, string>> items;
          private Context context;
          private MainActivity mainActivity;
          private List<View> rows;

          public MyListViewAdapter(MainActivity activity, Context context, List<Android.Net.Uri> images, List<string> messages) 
          {
               this.context = context;
               this.items = new List<Tuple<Android.Net.Uri, string>>();
               this.mainActivity = activity;
               this.rows = new List<View>();
               for (int i = 0; i < images.Count; ++i) 
               {
                    if (i < messages.Count) 
                    {
                         items.Add(new Tuple<Android.Net.Uri, string>(images[i], messages[i]));
                    }
               }
          }
          public override Tuple<Android.Net.Uri, string> this[int position] => items[position];

          public override int Count => items.Count;

          public override long GetItemId(int position)
          {
               return position;
          }

          public override View GetView(int position, View convertView, ViewGroup parent)
          {
               View row = rows[position];

               if (row == null) 
               {
                    row = LayoutInflater.From(context).Inflate(Resource.Layout.ListView_Row, null, false);
               }

               TextView text = row.FindViewById<TextView>(Resource.Id.text);
               text.Text = items[position].Item2;


               return row;
          }

          public int Add(Android.Net.Uri uri) 
          {
               items.Add(new Tuple<Android.Net.Uri, string>(uri, ""));
               View view = LayoutInflater.From(context).Inflate(Resource.Layout.ListView_Row, null, false);
               ImageView imageView = view.FindViewById<ImageView>(Resource.Id.image);

               string path = GetRealPathFromURI(uri);
               BitmapFactory.Options options = GetBitmapOptionsOfImage(path);
               Bitmap bitmapToDisplay = LoadScaledDownBitmapForDisplay(path, options, 100, 100);

               imageView.SetImageBitmap(bitmapToDisplay);
               rows.Add(view);
               return items.Count - 1;
          }

          private string GetRealPathFromURI(Android.Net.Uri contentURI)
          {
               ICursor cursor = mainActivity.ContentResolver.Query(contentURI, null, null, null, null);
               cursor.MoveToFirst();
               string documentId = cursor.GetString(0);
               documentId = documentId.Split(':')[1];
               cursor.Close();

               cursor = mainActivity.ContentResolver.Query(
               Android.Provider.MediaStore.Images.Media.ExternalContentUri,
               null, MediaStore.Images.Media.InterfaceConsts.Id + " = ? ", new[] { documentId }, null);
               cursor.MoveToFirst();
               string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data));
               cursor.Close();

               return path;
          }

          BitmapFactory.Options GetBitmapOptionsOfImage(string imagePath)
          {
               BitmapFactory.Options options = new BitmapFactory.Options
               {
                    InJustDecodeBounds = true
               };

               Bitmap bitmap = BitmapFactory.DecodeFile(imagePath, options);


               return options;
          }

          private int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
          {
               // Raw height and width of image
               float height = options.OutHeight;
               float width = options.OutWidth;
               double inSampleSize = 1D;

               if (height > reqHeight || width > reqWidth)
               {
                    int halfHeight = (int)(height / 2);
                    int halfWidth = (int)(width / 2);

                    // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                    while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                    {
                         inSampleSize *= 2;
                    }
               }

               return (int)inSampleSize;
          }

          private Bitmap LoadScaledDownBitmapForDisplay(string path, BitmapFactory.Options options, int reqWidth, int reqHeight)
          {
               // Calculate inSampleSize
               options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

               // Decode bitmap with inSampleSize set
               options.InJustDecodeBounds = false;

               return BitmapFactory.DecodeFile(path, options);
          }

          public void ChangeMessage(int position, string message) 
          {
               items[position] = new Tuple<Android.Net.Uri, string>(items[position].Item1, message);
               this.NotifyDataSetChanged();
          }

          

     }
}