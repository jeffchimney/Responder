using System;
using System.Collections.Generic;
using System.Net.Http;

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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Java.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.CognitoIdentity;
using Android.Gms.Common;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]

namespace Responder.Droid
{
	[Activity(Label = "Responder.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, SettingsTabInterface, GetLocationInterface
    {
        firehall.net_https.WebService1 responder = new firehall.net_https.WebService1();
		LocationTracker locationTracker = null;
		double latitude = 0;
		double longitude = 0;
		Decimal? dHallLong = 0;
		Decimal? dHallLat = 0;
		double TimeToHall = -1;
        private static readonly HttpClient client = new HttpClient();
        AmazonSimpleNotificationServiceClient snsClient;
        bool responding = false;

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

            snsClient = new AmazonSimpleNotificationServiceClient("AKIAJG5P2JQN2CRRM2IQ", "6mdlnDzPFC3wry1K78eC+9Gz15FnWDGFeO2tRFwt", RegionEndpoint.USWest2);

            LoadApplication(new App());
		}

		protected override void OnResume()
		{
			base.OnResume();
			locationTracker = new LocationTracker(this, false);
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
            if (HasNetworkConnectivity()) {
                Console.WriteLine(Build.Serial);
                var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);

                var sAccountInfo = GetAccountInfoFromUserDefaults();
                firehall.net_https.WS_Output result;
                if (sAccountInfo == ":")
                { // register
                    Guid deviceID = Guid.NewGuid();
                    localSettings.Edit().PutString("DeviceId", deviceID.ToString()).Commit();
                    result = responder.Register(0, 2, sFirehallID, sUserID, deviceID.ToString());
                }
                else // login
                {
                    String deviceID = GetUniqueDeviceID();
                    if (deviceID == "")
                    {
                        deviceID = Guid.NewGuid().ToString();
                        localSettings.Edit().PutString("DeviceId", deviceID).Commit();
                        result = responder.Register(0, 2, sFirehallID, sUserID, deviceID);
                    }
                    else
                    {
                        result = responder.Login(0, 2, sFirehallID, sUserID, deviceID);
                    }
                    if (result.ErrorMessage == "Device not yet registered")
                    {
                        result = responder.Register(0, 2, sFirehallID, sUserID, GetUniqueDeviceID());
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
            } else {
                return "No Connection";
            }
		}

        public string GetUniqueDeviceID()
        {
            var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.Private);

            String sDeviceID = localSettings.GetString("DeviceId", "");
            return sDeviceID;
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

        public bool CheckAuthorizationStatus() {
            return true;
        }

		public string GetLocation()
		{
            if (HasNetworkConnectivity())
            {
                // pass in the provider (GPS), 
                // the minimum time between updates (in seconds), 
                // the minimum distance the user needs to move to generate an update (in meters),
                // and an ILocationListener
                //var lastKnownLocation = locMgr.GetLastKnownLocation(LocationManager.GpsProvider);

                Decimal lastLat = Decimal.Parse(GetLastLatitude());
                Decimal lastLong = Decimal.Parse(GetLastLongitude());

                // CALCULATE TRAVEL TIME

                var response = responder.Responding(0, 2, GetUniqueDeviceID(), lastLat, lastLong, (int)TimeToHall);

                dHallLat = response.HallLatitude;
                dHallLong = response.HallLongitude;


                if (GetLastLatitude() != "0" && GetLastLongitude() != "0")
                {
                    CalculateTravelTimeBetween(double.Parse(GetLastLatitude()), double.Parse(GetLastLongitude()), (double)dHallLat, (double)dHallLong);
                }

                return response.Result.ToString();
            } else {
                return "No Connection";
            }
		}

		public List<ResponderResult> GetAllResponders()
		{
            if (HasNetworkConnectivity())
            {
                var responderList = new List<ResponderResult>();

                //GetLocation();
                var response = responder.GetResponses(0, 2, GetUniqueDeviceID());

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

                    foreach (firehall.net_https.WS_Response additionalResponse in response.Responses)
                    {
                        responderList.Add(new ResponderResult(additionalResponse.FullName, additionalResponse.DistanceToHall, additionalResponse.TimeToHall));
                    }
                }
                return responderList;
            } else {
                // no connection
                return new List<ResponderResult>();
            }
		}

		public void LinkToSettings()
		{

		}

		public async void CalculateTravelTimeBetween(double lat1, double long1, double lat2, double long2)
		{
            if (HasNetworkConnectivity())
            {
                var origin = new LatLng(lat1, long1);
                var dest = new LatLng(lat2, long2);
                var responseString = await client.GetStringAsync(GetDirectionsUrl(origin, dest));

                var jsonResult = JObject.Parse(responseString);
                var sTimeToHallResult = jsonResult["rows"][0]["elements"][0]["status"];

                if (sTimeToHallResult.ToString() != "ZERO_RESULTS")
                {
                    var sSecondsToHall = jsonResult["rows"][0]["elements"][0]["duration"]["value"].ToString();
                    var dSecondsToHall = double.Parse(sSecondsToHall);
                    TimeToHall = dSecondsToHall;
                }
            }
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
            if (HasNetworkConnectivity())
            {
                responder.SetStatusNR(0, 2, GetUniqueDeviceID());
            }
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

		private String GetDirectionsUrl(LatLng origin, LatLng dest)
		{

			// Origin of route
			String str_origin = origin.Latitude + "," + origin.Longitude;

			// Destination of route
			String str_dest = dest.Latitude + "," + dest.Longitude;

			// Sensor enabled
			//String sensor = "sensor=false";

			//// Building the parameters to the web service
			//String parameters = str_origin + "&" + str_dest + "&" + sensor;

			//// Output format
			//String output = "json";

			// Building the url to the web service

			String url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + str_origin + "&destinations=" + str_dest +"&key=AIzaSyDEwhj5NF6QkIOyTwpEc43cresueUK8sSs";

			return url;
		}

		public void CallToHall(string sTitle, string sMessage)
		{
            //if (HasNetworkConnectivity())
            //{
            //    PublishNotificationWithMessage(sTitle, sMessage);
            //}
		}

		public bool HasNetworkConnectivity()
		{
			if (CrossConnectivity.Current.IsConnected)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public void SetResponding(bool isResponding)
        {
            responding = isResponding;
        }
	}
}
