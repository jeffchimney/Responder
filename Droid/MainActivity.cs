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
using Android;
using Android.Util;
using Android.Support.V4.App;
using Android.Support.Design.Widget;

using Java.Interop;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]

namespace Responder.Droid
{
	[Activity(Label = "Responder.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, SettingsTabInterface, GetLocationInterface
	{
		firehall.net.WebService1 responder = new firehall.net.WebService1();
		LocationTracker locationTracker = null;
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
			locationTracker = new LocationTracker(this, true);
			locationTracker.InitializeLocationManager(true);
			locationTracker.LocationChanged += LocationTracker_LocationChanged;
            Location currentLocation = locationTracker.CurrentLocation;
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
				if (result.ErrorMessage == "Device not yet registered")
				{
					result = responder.Register(0, 1, sFirehallID, sUserID, Settings.Secure.AndroidId);
				}
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

            Decimal lastLat = Decimal.Parse(GetLastLatitude());
            Decimal lastLong = Decimal.Parse(GetLastLongitude());

			// CALCULATE TRAVEL TIME

			var response = responder.Responding(0, 1, Settings.Secure.AndroidId, lastLat, lastLong, (int)TimeToHall);

			dHallLat = response.HallLatitude;
			dHallLong = response.HallLongitude;

			return response.Result.ToString();
		}

		public List<ResponderResult> GetAllResponders()
		{
			var responderList = new List<ResponderResult>();

			//GetLocation();
			var response = responder.GetResponses(0, 1, Settings.Secure.AndroidId);

			dHallLat = response.HallLatitude;
			dHallLong = response.HallLongitude;

			if (response.ErrorMessage == "Device not found")
			{
				return new List<ResponderResult>();
			}

			if (response != null)
			{
				if (response.MyResponse != null)
				{
                    // calculate travel time to hall
                    //var myCoordinates = new CLLocationCoordinate2D(latitude, longitude);
                    //Console.WriteLine("My Coordinates: " + myCoordinates.ToString());
                    //var hallCoordinates = new CLLocationCoordinate2D((double)dHallLat, (double)dHallLong);
                    //Console.WriteLine("Hall Coordinates: " + hallCoordinates.ToString());
                    CalculateTravelTimeBetween(double.Parse(GetLastLatitude()), double.Parse(GetLastLongitude()), (double)dHallLat, (double)dHallLong);

					var sTimeToHall = "";
					if ((int)TimeToHall != -1)
					{
						sTimeToHall = response.MyResponse.TimeToHall;
					}

					var myResponse = new ResponderResult(response.MyResponse.FullName, response.MyResponse.DistanceToHall, response.MyResponse.TimeToHall);
					//var myResponse = new ResponderResult(mySeparateResponse.MyResponse.FullName, mySeparateResponse.MyResponse.DistanceToHall, sTimeToHall);
					responderList.Add(myResponse);
				}
				else // add an empty response to show you are not currently responding.
				{
					var myResponse = new ResponderResult("Not Responding", " ", "N/R");
					responderList.Add(myResponse);
				}

				foreach (firehall.net.WS_Response additionalResponse in response.Responses)
				{
					responderList.Add(new ResponderResult(additionalResponse.FullName, additionalResponse.DistanceToHall, additionalResponse.TimeToHall));
				}
			}
			return responderList;
		}

		public void LinkToSettings()
		{

		}

		public void CalculateTravelTimeBetween(double lat1, double long1, double lat2, double long2)
		{
			//var sourcePlacemark = new MKPlacemark(coord1);
			//var sourceMapItem = new MKMapItem(sourcePlacemark);

			//var destinationPlacemark = new MKPlacemark(coord2);
			//var destinationMapItem = new MKMapItem(destinationPlacemark);

			//var request = new MKDirectionsRequest();
			//request.Source = sourceMapItem;
			//request.Destination = destinationMapItem;
			//request.TransportType = MKDirectionsTransportType.Automobile;
			//request.RequestsAlternateRoutes = false;

			//var directions = new MKDirections(request);

			//directions.CalculateDirections((response, error) =>
			//{
			//	if (error != null)
			//	{
			//		Console.WriteLine("Error with maps api");
			//	}
			//	else
			//	{
			//		var route = response.Routes.FirstOrDefault();
			//		TimeToHall = route.ExpectedTravelTime;
			//		Console.WriteLine("Minutes to hall: " + route.ExpectedTravelTime.ToString());
			//	}
			//});
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

		private void LocationTracker_LocationChanged(object sender, Location e)
		{
            var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);
            localSettings.Edit().PutString("Latitude", e.Latitude.ToString());
            localSettings.Edit().PutString("Longitude", e.Longitude.ToString());

			Console.WriteLine("Location changed");
			Console.WriteLine("Latitude: " + latitude.ToString());
			Console.WriteLine("Longitude: " + longitude.ToString());
		}

        private string GetLastLatitude() {
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.WorldWriteable);
			var sLastLatitude = localSettings.GetString("Latitude", "0");

			return sLastLatitude;
        }

        private string GetLastLongitude() {
			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.WorldWriteable);
			var sLastLongitude = localSettings.GetString("Longitude", "0");

			return sLastLongitude;
        }
	}
}
