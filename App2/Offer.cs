using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace App2
{
    [Activity(Label = "Activity1")]
    public class OfferActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.offer_layout);

            var data = Intent.GetStringExtra("offerInfo") ?? String.Empty;
            var Label = FindViewById<TextView>(Resource.Id.textHolder);
            Label.Text = data;
        }
    }
}