using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreLocation;
using Responder.iOS;
[assembly: Xamarin.Forms.Dependency(typeof(Application))]

namespace Responder.iOS
{
	public class Application: GetLocationInterface, SettingsTabInterface
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}

		// Get Location Interface Methods
		public string GetLocation()
		{
			CLLocationManager locationManager = new CLLocationManager();
			locationManager.RequestAlwaysAuthorization();

			Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
			Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

			firehall.net.WebService1 responder = new firehall.net.WebService1();
			Console.WriteLine(locationManager.Location.Coordinate.Latitude + ", " + locationManager.Location.Coordinate.Longitude);

			return responder.Responding(UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude);
		}

		public bool AskForLocationPermissions()
		{
			CLLocationManager locationManager = new CLLocationManager();
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
		public void SubmitAccountInfo(string sFirehallID, string sUserID)
		{
			firehall.net.WebService1 responder = new firehall.net.WebService1();

			string result = responder.Register(sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());

			if (result == string.Empty || result == "device already registered")
			{
				// save deviceID to userdefaults
				var defaults = NSUserDefaults.StandardUserDefaults;

				defaults.SetString(sFirehallID, "FireHallID");
				defaults.SetString(sUserID, "UserID");
				defaults.Synchronize();
			}
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
	}
}
