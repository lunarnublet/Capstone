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

namespace AndroidApplication
{
     class MyListViewAdapter : BaseAdapter<Tuple<Android.Net.Uri, string>>
     {
          private List<Tuple<Android.Net.Uri, string>> items;
          private Context context;
          private MainActivity mainActivity;

          public MyListViewAdapter(MainActivity activity, Context context, List<Android.Net.Uri> images, List<string> messages) 
          {
               this.context = context;
               this.items = new List<Tuple<Android.Net.Uri, string>>();
               this.mainActivity = activity;
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
               View row = convertView;

               if (row == null) 
               {
                    row = LayoutInflater.From(context).Inflate(Resource.Layout.ListView_Row, null, false);
               }

               ImageView imageView = row.FindViewById<ImageView>(Resource.Id.image);
               imageView.SetImageURI(items[position].Item1);

               TextView text = row.FindViewById<TextView>(Resource.Id.text);
               text.Text = items[position].Item2;


               return row;
          }

          public int Add(Android.Net.Uri uri) 
          {
               items.Add(new Tuple<Android.Net.Uri, string>(uri, ""));
               return items.Count - 1;
          }

          public void ChangeMessage(int position, string message) 
          {
               items[position] = new Tuple<Android.Net.Uri, string>(items[position].Item1, message);
               this.NotifyDataSetChanged();
          }

          

     }
}