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
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]

namespace Responder.Droid
{
	[Activity(Label = "Responder.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, SettingsTabInterface, GetLocationInterface, ILocationListener
	{
		firehall.net.WebService1 responder = new firehall.net.WebService1();
		LocationManager locMgr;
		String locProvider;
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
			InitializeLocationManager();

			if (locMgr.IsProviderEnabled(locProvider))
			{
				locMgr.RequestLocationUpdates(locProvider, 2000, 1, this);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Could not resume location services.  Provider is not enabled.");
			}
		}

		void InitializeLocationManager()
		{
			locMgr = (LocationManager)GetSystemService(Context.LocationService);
			Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.Fine };
			try
			{
				var isGPSReady = locMgr.IsProviderEnabled(LocationManager.GpsProvider);
				var isNetworkReady = locMgr.IsProviderEnabled(LocationManager.NetworkProvider);

				locProvider = locMgr.GetBestProvider(criteriaForLocationService, true);
				if (locMgr.IsProviderEnabled(locProvider))
				{
					locMgr.RequestLocationUpdates(locProvider, 2000, 1, this);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		// Settings Tab Interface Method
		public string SubmitAccountInfo(string sFirehallID, string sUserID)
		{
			Console.WriteLine(Build.Serial);
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);

			var sAccountInfo = GetAccountInfoFromUserDefaults();
			firehall.net.WS_Output result;
			if (sAccountInfo == ":")
			{ // register
				result = responder.Register(0, 1, sFirehallID, sUserID, Settings.Secure.AndroidId);
			}
			else // login
			{
				result = responder.Login(0, 1, sFirehallID, sUserID, Settings.Secure.AndroidId);
			}

			if (result.Result.ToString() == "Upgrade")
			{
				//UIAlertView avAlert = new UIAlertView("Upgrade Required", "Responder requires an update.", null, "OK", null); // null replaces completion handler, should send us to the app store for an update.
				//avAlert.Show();
				return result.Result.ToString();
			}

			if (result.Result.ToString() == "OK" || result.Result.ToString() == "Admin" || result.Result.ToString() == "device already registered")
			{
				// save deviceID to Defaults
				localSettings.Edit().PutString("FirehallID", sFirehallID).Commit();
				localSettings.Edit().PutString("UserID", sUserID).Commit();

				if (result.Result.ToString() == "Admin")
				{
					localSettings.Edit().PutBoolean("IsAdmin", true).Commit();
				}
				else
				{
					localSettings.Edit().PutBoolean("IsAdmin", false).Commit();
				}
			}
			return result.Result.ToString();
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
			// pass in the provider (GPS), 
			// the minimum time between updates (in seconds), 
			// the minimum distance the user needs to move to generate an update (in meters),
			// and an ILocationListener
			//var lastKnownLocation = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

			Decimal lastLat = (Decimal)latitude;
			Decimal lastLong = (Decimal)longitude;

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
			var responderList = new List<ResponderResult>();

			GetLocation();
			var response = responder.GetResponses(0, 1, Settings.Secure.AndroidId);

			dHallLat = response.HallLatitude;
			dHallLong = response.HallLongitude;

			if (response.ErrorMessage == "Device not found")
			{
				return new List<ResponderResult>();
			}

			//if ((int)TimeToHall != -1)
			//{
			//    var separateResponse = responder.Responding(0, 1, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);
			//}

			if (response != null)
			{
				if (response.MyResponse != null)
				{
					// calculate travel time to hall
					//var myCoordinates = new CLLocationCoordinate2D(latitude, longitude);
					//Console.WriteLine("My Coordinates: " + myCoordinates.ToString());
					//var hallCoordinates = new CLLocationCoordinate2D((double)dHallLat, (double)dHallLong);
					//Console.WriteLine("Hall Coordinates: " + hallCoordinates.ToString());
					//CalculateTravelTimeBetween(myCoordinates, hallCoordinates);

					var sTimeToHall = "";
					if ((int)TimeToHall != -1)
					{
						sTimeToHall = response.MyResponse.TimeToHall;
					}
					//else
					//{
					//  sTimeToHall = TimeToHall.ToString();
					//}

					var myResponse = new ResponderResult(response.MyResponse.FullName, GetDistanceFromHall(), GetTimeToHall());
					//var myResponse = new ResponderResult(mySeparateResponse.MyResponse.FullName, mySeparateResponse.MyResponse.DistanceToHall, sTimeToHall);
					responderList.Add(myResponse);
				}
				else // add an empty response to show you are not currently responding.
				{
					var myResponse = new ResponderResult("Not Responding", " ", "N/A");
					responderList.Add(myResponse);
				}

				foreach (firehall.net.WS_Response additionalResponse in response.Responses)
				{
					responderList.Add(new ResponderResult(additionalResponse.FullName, additionalResponse.DistanceToHall, additionalResponse.TimeToHall));
				}
			}
			return responderList;
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
			responder.SetStatusNR(0, 1, Settings.Secure.AndroidId);
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
		public bool IsAdmin()
		{
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);
			var bIsAdmin = localSettings.GetBoolean("IsAdmin", false);

			return bIsAdmin;
		}

		public string GetTimeToHall()
		{
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);
			var sTimeToHall = localSettings.GetString("TimeToHall", "");

			return sTimeToHall;
		}

		public string GetDistanceFromHall()
		{
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);
			var sDistanceFromHall = localSettings.GetString("DistanceFromHall", "");

			return sDistanceFromHall;
		}
	}
}
