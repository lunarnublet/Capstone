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
     public class MyListView : ListActivity
     {
          protected override void OnCreate(Bundle savedInstanceState)
          {
               base.OnCreate(savedInstanceState);

               //ListAdapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, country);

               //ListView.TextFilterEnabled = true;

               //ListView.ItemClick += delegate (object sender, ItemEventArgs args) {
               //     // When clicked, show a toast with the TextView text
               //     Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
               //};
          }
     }
}