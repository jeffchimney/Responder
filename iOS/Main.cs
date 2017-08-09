using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;
using Responder.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(Application))]

namespace Responder.iOS
{
	public class Application: GetLocationInterface, SettingsTabInterface
	{
		CLLocationManager locationManager = new CLLocationManager();

		Decimal? dHallLong = 0;
		Decimal? dHallLat = 0;
		double TimeToHall = -1;

		firehall.net.WebService1 responder = new firehall.net.WebService1();

		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}

		public void StartListening()
		{
			locationManager.RequestAlwaysAuthorization();
			locationManager.StartUpdatingLocation();
		}

		// Get Location Interface Methods
		public string GetLocation()
		{
			Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
			Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

			Console.WriteLine(locationManager.Location.Coordinate.Latitude + ", " + locationManager.Location.Coordinate.Longitude);

			// calculate distance to hall
			var myCoordinates = new CLLocationCoordinate2D((double)latitude, (double)longitude);
			var hallCoordinates = new CLLocationCoordinate2D((double)dHallLat, (double)dHallLong);
			CalculateTravelTimeBetween(myCoordinates, hallCoordinates);

			var response = responder.Responding(0, 1, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);

			dHallLong = response.HallLongitude;
			dHallLat = response.HallLatitude;

			return response.Result.ToString();
		}

		public List<ResponderResult> GetAllResponders()
		{
			Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
			Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

			var response = responder.GetResponses(0, 1, UIDevice.CurrentDevice.IdentifierForVendor.ToString());

			dHallLat = response.HallLatitude;
			dHallLong = response.HallLongitude;

			var responderList = new List<ResponderResult>();

			if (response.ErrorMessage == "Device not found")
			{
				return new List<ResponderResult>();
			}

			if (response != null)
			{
				if (response.MyResponse != null)
				{
					// calculate travel time to hall
					var myCoordinates = new CLLocationCoordinate2D((double)latitude, (double)longitude);
					Console.WriteLine("My Coordinates: " + myCoordinates.ToString());
					var hallCoordinates = new CLLocationCoordinate2D((double)dHallLat, (double)dHallLong);
					Console.WriteLine("Hall Coordinates: " + hallCoordinates.ToString());
					CalculateTravelTimeBetween(myCoordinates, hallCoordinates);

					var sTimeToHall = "";
					if ((int)TimeToHall == -1)
					{
						sTimeToHall = response.MyResponse.TimeToHall;
					}
					else
					{
						sTimeToHall = TimeToHall.ToString();
					}

					var myResponse = new ResponderResult(response.MyResponse.FullName, response.MyResponse.DistanceToHall, sTimeToHall);
					responderList.Add(myResponse);
				}
				else // add an empty response to show you are not currently responding.
				{
					var myResponse = new ResponderResult("Not Responding", " ", "N/A");
					responderList.Add(myResponse);
				}

				foreach (firehall.net.WS_Response additionalResponse in response.Responses)
				{
					responderList.Add(new ResponderResult(additionalResponse.FullName, additionalResponse.DistanceToHall, additionalResponse.TimeToHall ?? " "));
				}
			}
			return responderList;
		}

		public void CalculateTravelTimeBetween(CLLocationCoordinate2D coord1, CLLocationCoordinate2D coord2)
		{
			var sourcePlacemark = new MKPlacemark(coord1);
			var sourceMapItem = new MKMapItem(sourcePlacemark);

			var destinationPlacemark = new MKPlacemark(coord2);
			var destinationMapItem = new MKMapItem(destinationPlacemark);

			var request = new MKDirectionsRequest();
			request.Source = sourceMapItem;
			request.Destination = destinationMapItem;
			request.TransportType = MKDirectionsTransportType.Automobile;
			request.RequestsAlternateRoutes = false;

			var directions = new MKDirections(request);

			directions.CalculateDirections((response, error) => 
			{
				if (error != null)
				{
					Console.WriteLine("Error with maps api");
				}
				else
				{
					var route = response.Routes.FirstOrDefault();
					TimeToHall = route.ExpectedTravelTime;
					Console.WriteLine("Minutes to hall: " + route.ExpectedTravelTime.ToString());
				}
			});
		}

		public void StartMonitoringLocationInBackground()
		{
			Console.WriteLine("Start monitoring in background");
			locationManager.RequestAlwaysAuthorization();
			locationManager.DesiredAccuracy = 1;

			if (UIDevice.CurrentDevice.IsMultitaskingSupported)
			{
				Console.WriteLine("Multitasking supported");
				// Code dependent on multitasking.
				if (CLLocationManager.LocationServicesEnabled)
				{
					Console.WriteLine("Location services enabled");
					locationManager.StartMonitoringSignificantLocationChanges();

					locationManager.LocationsUpdated += (o, e) =>
					{
						Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
						Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);
						firehall.net.WebService1 responder = new firehall.net.WebService1();
						Console.WriteLine("Noticed change in location");
						var result = responder.Responding(0, 1, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);
						Console.WriteLine(result);
						if (result.Result.ToString().Contains("DONE"))
						{
							StopMonitoringLocationChanges();
							locationManager.StopUpdatingLocation();
						}
					};
				}
				else {
					Console.WriteLine("Location services not enabled, please enable this in your Settings");
				}
			}
		}

		public void StopMonitoringLocationChanges()
		{
			locationManager.StopMonitoringSignificantLocationChanges();
		}

		public void StopListening()
		{
            responder.SetStatusNR(0, 1, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
			//responder.StopResponding(0, 1, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
		}

		public bool AskForLocationPermissions()
		{
			locationManager.RequestAlwaysAuthorization();

			var bLocationEnabled = false;

			locationManager.AuthorizationChanged += (object sender, CLAuthorizationChangedEventArgs e) =>
			{
				if (e.Status == CLAuthorizationStatus.Denied || e.Status == CLAuthorizationStatus.Restricted)
				{
					bLocationEnabled = false;
				}
				else if (e.Status == CLAuthorizationStatus.Authorized)
				{
					bLocationEnabled = true;
					// save deviceID to userdefaults
					var defaults = NSUserDefaults.StandardUserDefaults;

					defaults.SetString(bLocationEnabled.ToString(), "bLocationEnabled");
					defaults.Synchronize();
				}
			};

			return bLocationEnabled;
		}

		public void RegisterForPushNotifications()
		{
			// register for notifications
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
								   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
								   new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			else {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
			}
		}

		// Settings Tab Interface Method
		public string SubmitAccountInfo(string sFirehallID, string sUserID)
		{
            var sAccountInfo = GetAccountInfoFromUserDefaults();
            firehall.net.WS_Output result;
            if (sAccountInfo == ":")
            { // register
                result = responder.Register(0, 1, sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
            }
            else // login
            {
                result = responder.Login(0, 1, sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
            }

            if (result.Result.ToString() == "Upgrade") {
				UIAlertView avAlert = new UIAlertView("Upgrade Required", "Responder requires an update.", null, "OK", null); // null replaces completion handler, should send us to the app store for an update.
				avAlert.Show();
                return result.Result.ToString();
			}

			if (result.Result.ToString() == "OK" || result.Result.ToString() == "Admin" ||  result.Result.ToString() == "device already registered")
			{
				// save deviceID to userdefaults
				var defaults = NSUserDefaults.StandardUserDefaults;

				defaults.SetString(sFirehallID, "FireHallID");
				defaults.SetString(sUserID, "UserID");
                if (result.Result.ToString() == "Admin") {
                    defaults.SetBool(true, "IsAdmin");
                } else {
                    defaults.SetBool(false, "IsAdmin");
                }
				defaults.Synchronize();
			}
            return result.Result.ToString();
		}

		// Settings Tab Interface Method
		public string GetAccountInfoFromUserDefaults()
		{
			// get account info from userdefaults
			var defaults = NSUserDefaults.StandardUserDefaults;

			string sFireHallID = defaults.StringForKey("FireHallID");
			string sUserID = defaults.StringForKey("UserID");

			return sFireHallID + ":" + sUserID;
		}

        public bool IsAdmin() {
			// get account info from userdefaults
			var defaults = NSUserDefaults.StandardUserDefaults;

            bool bIsAdmin = defaults.BoolForKey("IsAdmin");
            return bIsAdmin;
        }
	}
}
