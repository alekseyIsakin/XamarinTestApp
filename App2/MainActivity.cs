using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using AndroidX.AppCompat.App;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Security;

namespace App2
{
    public struct Offer
    {
        public string ID;
        public string InnerXml;
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private HttpClient client = new HttpClient();

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string url = "https://yastatic.net/market-export/_/partner/help/YML.xml";

            Task<Stream> dataStream = FetchTestDataAsync(url);
            List<Offer> data = new List<Offer>(){ };

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            try
            {
                Stream dt = await dataStream;
                data = await FetchTestDataAsync(dt);
            }catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            ListView l = FindViewById<ListView>(Resource.Id.listView1);
            var adapter = new CustomListAdapter(this, data);
            l.Adapter = adapter;

            l.ItemClick += (sender, e) =>
            {
                Offer select = adapter[e.Position];
                var intent = new Intent(this, typeof(OfferActivity));
                intent.PutExtra("offerInfo", select.InnerXml);

                StartActivity(intent);
            };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected async Task<List<Offer>> FetchTestDataAsync(Stream xmlStream)
        {
            List<Offer> res = new List<Offer>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(xmlStream, settings);

            while (await reader.ReadAsync())
            {

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        System.Diagnostics.Debug.WriteLine(reader.Name);
                        if (reader.Name != "offer")
                            break;
                        var id = reader.GetAttribute("id");
                        var innerXml = reader.ReadInnerXml();

                        Offer offer = new Offer()
                        {
                            ID = id,
                            InnerXml = innerXml
                        };
                        res.Add(offer);
                        break;
                }
            }
            return res;
        }
        protected async Task<Stream> FetchTestDataAsync(string url)
        {
            var resp = await client.GetAsync(url);
            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsStreamAsync().Result;
            }
            return Stream.Null;
        }
    }



    public class CustomListAdapter : BaseAdapter<Offer>
    {
        private List<Offer> items;
        private Context context;

        public CustomListAdapter(Context context, List<Offer> items) {
            this.items = items;
            this.context = context;
        }
        public override Offer this[int position] => items[position];
        public override int Count => items.Count;
        public override long GetItemId(int position) => position;
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(context).Inflate(Resource.Layout.list_element, null);
            }

            TextView tv = view.FindViewById<TextView>(Resource.Id.textView);
            tv.Text = items[position].ID;

            return view;
        }
    }

}