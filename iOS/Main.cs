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

		// Get Location Interface Method
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

		// Settings Tab Interface Method
		public void SubmitAccountInfo(string sFirehallID, string sUserID)
		{
			firehall.net.WebService1 responder = new firehall.net.WebService1();

			string result = responder.Register(sFirehallID, sUserID, UIDevice.CurrentDevice.IdentifierForVendor.ToString());

			if (result == string.Empty)
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
