using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Responder.Droid;
[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]

namespace Responder.Droid
{
	[Activity(Label = "Responder.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, SettingsTabInterface, GetLocationInterface
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}

		// Settings Tab Interface Method
		public void SubmitAccountInfo(string sFirehallID, string sUserID)
		{
			//firehall.net.WebService1 responder = new firehall.net.WebService1();

			//Object result = responder.Register(1, 0, sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());

			//if (result == string.Empty || result == "device already registered")
			//{
			//	// save deviceID to userdefaults
			//	var defaults = NSUserDefaults.StandardUserDefaults;

			//	defaults.SetString(sFirehallID, "FireHallID");
			//	defaults.SetString(sUserID, "UserID");
			//	defaults.Synchronize();
			//}
		}


		// MARK: GetLocation Interface Methods

		public string GetAccountInfoFromUserDefaults()
		{
			// get account info from userdefaults
			// var defaults = NSUserDefaults.StandardUserDefaults;

			//string sFireHallID = defaults.StringForKey("FireHallID");
			//string sUserID = defaults.StringForKey("UserID");

			return "" + ":" + "";
		}

		// MARK: GetLocation Interface Methods

		public string GetLocation()
		{
			return "";
		}

		public List<ResponderResult> GetAllResponders()
		{
			return new List<ResponderResult>();
		}

		public bool AskForLocationPermissions()
		{
			return true;
		}

		public void RegisterForPushNotifications()
		{

		}

		public void StartMonitoringLocationInBackground()
		{

		}

		public void StopMonitoringLocationChanges()
		{
			
		}

		public void StartListening()
		{

		}
	}
}
