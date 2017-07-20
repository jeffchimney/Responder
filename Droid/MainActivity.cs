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
		double dHallLong = 0;
		double dHallLat = 0;

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
//Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
//Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

//Console.WriteLine(locationManager.Location.Coordinate.Latitude + ", " + locationManager.Location.Coordinate.Longitude);

//			// calculate distance to hall
//			var myCoordinates = new CLLocationCoordinate2D((double)latitude, (double)longitude);
//var hallCoordinates = new CLLocationCoordinate2D((double)dHallLat, (double)dHallLong);
//			CalculateTravelTimeBetween(myCoordinates, hallCoordinates);

//var response = responder.Responding(1, 0, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);

//dHallLong = response.HallLongitude;
//			dHallLat = response.HallLatitude;

//			return response.Result.ToString();







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

			responder.Responding(0, 1, Settings.Secure.AndroidId,lastKnownLocation.Latitude, lastKnownLocation.Longitude, 

			return "";
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

		}

		public void StopListening()
		{

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
