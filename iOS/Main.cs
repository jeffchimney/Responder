using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using MapKit;
using CoreLocation;
using Responder.iOS;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.CognitoIdentity;
using Plugin.Connectivity;

[assembly: Xamarin.Forms.Dependency(typeof(Application))]

namespace Responder.iOS
{
	public class Application: GetLocationInterface, SettingsTabInterface
	{
		CLLocationManager locationManager = new CLLocationManager();

		Decimal? dHallLong = 0;
		Decimal? dHallLat = 0;
		double TimeToHall = -1;
        bool responding = false;
        Decimal lastLatitude = 0;
        Decimal lastLongitude = 0;


        firehall.net_https.WebService1 responder = new firehall.net_https.WebService1();

		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}

        public bool CheckAuthorizationStatus() {
            return CLLocationManager.LocationServicesEnabled;
        }

		public void StartListening()
		{
			locationManager.RequestAlwaysAuthorization();
            locationManager.RequestWhenInUseAuthorization();
            locationManager.StartUpdatingLocation();
		}

		// Get Location Interface Methods
		public string GetLocation()
		{
            locationManager.RequestAlwaysAuthorization();
            locationManager.RequestWhenInUseAuthorization();
            if (CLLocationManager.LocationServicesEnabled)
            {
                if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways || CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
                {
                    Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
                    Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

                    Console.WriteLine(locationManager.Location.Coordinate.Latitude + ", " + locationManager.Location.Coordinate.Longitude);

                    // calculate distance to hall
                    var myCoordinates = new CLLocationCoordinate2D((double)latitude, (double)longitude);
                    var hallCoordinates = new CLLocationCoordinate2D((double)dHallLat, (double)dHallLong);
                    CalculateTravelTimeBetween(myCoordinates, hallCoordinates);

                    if (HasNetworkConnectivity())
                    {
                        var response = responder.Responding(0, 2, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);

						SaveTimeToHall(response.MyResponse.TimeToHall);
						SaveDistanceFromHall(response.MyResponse.DistanceToHall);

						dHallLong = response.HallLongitude;
						dHallLat = response.HallLatitude;

						return response.Result.ToString();
                    }
                    else
                    {
                        return "No Connection";
                    }
                }
                else
                {
                    return "Location Services Not Enabled";
                }
            } else {
                return "Location Services Not Enabled";
            }
		}

		public List<ResponderResult> GetAllResponders()
		{
            var responderList = new List<ResponderResult>();
            if (locationManager.Location != null)
            {
                Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
                Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

                //GetLocation();
                var response = responder.GetResponses(0, 2, UIDevice.CurrentDevice.IdentifierForVendor.ToString());

                dHallLat = response.HallLatitude;
                dHallLong = response.HallLongitude;

                if (response.ErrorMessage == "Device not found")
                {
                    return new List<ResponderResult>();
                }

                //if ((int)TimeToHall != -1)
                //{
                //    var separateResponse = responder.Responding(0, 2, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);
                //}

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
                        if ((int)TimeToHall != -1)
                        {
                            sTimeToHall = response.MyResponse.TimeToHall;
                        }
                        //else
                        //{
                        //	sTimeToHall = TimeToHall.ToString();
                        //}

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
            }
			return responderList;
		}

        public void LinkToSettings() {
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var settingsString = UIKit.UIApplication.OpenSettingsUrlString;
				var url = new NSUrl(settingsString);
				UIApplication.SharedApplication.OpenUrl(url);
			}
        }

		public void CalculateTravelTimeBetween(CLLocationCoordinate2D coord1, CLLocationCoordinate2D coord2)
		{
            // MK items need to be run on main UI thread
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
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
            });
		}

        public void SetResponding(bool isResponding) {
            responding = isResponding;
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

                    locationManager.LocationsUpdated += (o, e) =>
                    {
                        Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
                        Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);
                        if (responding && (lastLongitude != longitude && lastLatitude != latitude))
                        {
                            lastLatitude = latitude;
                            lastLongitude = longitude;
                            firehall.net_https.WebService1 responder = new firehall.net_https.WebService1();
                            Console.WriteLine("Noticed change in location");
                            var result = responder.Responding(0, 2, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude, (int)TimeToHall);
                            Console.WriteLine(result);
                            if (result.Result.ToString().Contains("AtHall"))
                            {
                                StopMonitoringLocationChanges();
                                locationManager.StopUpdatingLocation();
                            }
                        }
                    };
                }
                else
                {
                    Console.WriteLine("Location services not enabled, please enable this in your Settings");
                }
            }
		}

		public void StopMonitoringLocationChanges()
		{
            locationManager.StopUpdatingLocation();
			locationManager.StopMonitoringSignificantLocationChanges();
		}

		public void StopListening()
		{
            StopMonitoringLocationChanges();
            responder.SetStatusNR(0, 2, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
		}

		public bool AskForLocationPermissions()
		{
			locationManager.RequestAlwaysAuthorization();
            locationManager.RequestWhenInUseAuthorization();

			var bLocationEnabled = false;

			locationManager.AuthorizationChanged += (object sender, CLAuthorizationChangedEventArgs e) =>
			{
                Console.Out.Write("Test");
				if (e.Status == CLAuthorizationStatus.Denied || e.Status == CLAuthorizationStatus.Restricted)
				{
					bLocationEnabled = false;
                    Console.Out.Write("Test2");
				}
				else if (e.Status == CLAuthorizationStatus.Authorized)
				{
                    Console.Out.Write("Test3");
					bLocationEnabled = true;
					// save deviceID to userdefaults
					var defaults = NSUserDefaults.StandardUserDefaults;

					defaults.SetBool(bLocationEnabled, "bLocationEnabled");
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
            firehall.net_https.WS_Output result;
            if (sAccountInfo == ":")
            { // register
                result = responder.Register(0, 2, sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
            }
            else // login
            {
                result = responder.Login(0, 2, sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
                if (result.ErrorMessage == "Device not yet registered"){
                    result = responder.Register(0, 2, sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());
                }
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

                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                appDelegate.RegisterForNotifications();
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

            object oIsAdmin = defaults.ValueForKey(NSObject.FromObject("IsAdmin") as NSString);
            bool bIsAdmin = false;
            if (oIsAdmin != null)
            {
                bIsAdmin = defaults.BoolForKey("IsAdmin");
            }
            return bIsAdmin;
        }

        public string GetTimeToHall() {
			var defaults = NSUserDefaults.StandardUserDefaults;

            string sTimeToHall = defaults.StringForKey("TimeToHall");
			return sTimeToHall;
        }

        public string GetDistanceFromHall() {
			var defaults = NSUserDefaults.StandardUserDefaults;

			string sDistanceFromHall = defaults.StringForKey("DistanceFromHall");
			return sDistanceFromHall;
        }

        public void SaveTimeToHall(string sTimeToHall)
        {
			var defaults = NSUserDefaults.StandardUserDefaults;

			defaults.SetString(sTimeToHall, "TimeToHall");
			defaults.Synchronize();
        }

        public void SaveDistanceFromHall(string sDistanceFromHall)
        {
			var defaults = NSUserDefaults.StandardUserDefaults;

			defaults.SetString(sDistanceFromHall, "DistanceFromHall");
			defaults.Synchronize();
        }

        public void CallToHall(string sTitle, string sMessage)
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            responder.PushNotification(0, 2, UIDevice.CurrentDevice.IdentifierForVendor.ToString(), "Test Notification");
            //appDelegate.PublishNotificationWithMessage(sTitle, sMessage);
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
	}
}