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
using Android.Provider;
using Android.Locations;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]

namespace Responder.Droid
{
	[Activity(Label = "Responder.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, SettingsTabInterface, GetLocationInterface, ILocationListener
	{
		firehall.net.WebService1 responder = new firehall.net.WebService1();
		LocationManager locMgr;
		double latitude = 0;
		double longitude = 0;
		Decimal? dHallLong = 0;
		Decimal? dHallLat = 0;
        double TimeToHall = -1;

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}

		protected override void OnResume()
		{
			base.OnResume();

			if (locMgr == null)
			{
				// initialize location manager
				locMgr = GetSystemService(LocationService) as LocationManager;
			}
		}

		// Settings Tab Interface Method
		public void SubmitAccountInfo(string sFirehallID, string sUserID)
		{
			Console.WriteLine(Build.Serial);
			var result = responder.Register(0, 1, sFirehallID, sUserID, Settings.Secure.AndroidId);
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);

			if (result.Result.ToString() == string.Empty || result.Result.ToString() == "device already registered")
			{
				// save deviceID to Defaults
				localSettings.Edit().PutString("FirehallID", sFirehallID).Commit();
				localSettings.Edit().PutString("UserID", sUserID).Commit();
			}
		}


		// MARK: GetLocation Interface Methods

		public string GetAccountInfoFromUserDefaults()
		{
			// get account info from userdefaults
			string sFireHallID;
			string sUserID;
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);

			sFireHallID = localSettings.GetString("FirehallID", "");
			sUserID = localSettings.GetString("UserID", "");

			return sFireHallID + ":" + sUserID;
		}

		// MARK: GetLocation Interface Methods

		public string GetLocation()
		{
			if (locMgr == null)
			{
				// initialize location manager
				locMgr = GetSystemService(LocationService) as LocationManager;
			}

			// pass in the provider (GPS), 
			// the minimum time between updates (in seconds), 
			// the minimum distance the user needs to move to generate an update (in meters),
			// and an ILocationListener
			if (locMgr.AllProviders.Contains (LocationManager.GpsProvider)
				&& locMgr.IsProviderEnabled (LocationManager.GpsProvider)) {
				locMgr.RequestLocationUpdates (LocationManager.GpsProvider, 30, 1, this);
			} else {
				Toast.MakeText (this, "The Network Provider does not exist or is not enabled!", ToastLength.Long).Show ();
			}
			var lastKnownLocation = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

            Decimal lastLat = (Decimal)lastKnownLocation.Latitude;
            Decimal lastLong = (Decimal)lastKnownLocation.Longitude;

            // CALCULATE TRAVEL TIME

            var response = responder.Responding(0, 1, Settings.Secure.AndroidId, lastLat, lastLong, (int)TimeToHall);

            dHallLat = response.HallLatitude;
            dHallLong = response.HallLongitude;

			return response.Result.ToString();
		}

		public void OnLocationChanged(Location location)
		{
			latitude = location.Latitude;
			longitude = location.Longitude;

			Console.WriteLine("Location changed");
			Console.WriteLine("Latitude: " + latitude.ToString());
			Console.WriteLine("Longitude: " + longitude.ToString());
			Console.WriteLine("Provider: " + location.Provider);
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
			//locationManager.RequestAlwaysAuthorization();
			//locationManager.StartUpdatingLocation();
		}

		public void StopListening()
		{
            responder.StopResponding(0, 1, Settings.Secure.AndroidId);
		}

		public void OnProviderDisabled(string provider)
		{
			Console.WriteLine(provider + " disabled by user");
		}
		public void OnProviderEnabled(string provider)
		{
			Console.WriteLine(provider + " enabled by user");
		}
		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
			Console.WriteLine(provider + " availability has changed to " + status.ToString());
		}
	}
}
