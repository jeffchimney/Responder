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
	public class Application: GetLocationInterface
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");

		}

		public void GetLocation()
		{
			CLLocationManager locationManager = new CLLocationManager();
			locationManager.RequestAlwaysAuthorization();

			Decimal latitude = Convert.ToDecimal(locationManager.Location.Coordinate.Latitude);
			Decimal longitude = Convert.ToDecimal(locationManager.Location.Coordinate.Longitude);

			firehall.net.WebService1 responder = new firehall.net.WebService1();
			responder.Responding("Test From Device Co.", UIDevice.CurrentDevice.IdentifierForVendor.ToString(), latitude, longitude);

			Console.WriteLine(locationManager.Location.Coordinate.Latitude + ", " + locationManager.Location.Coordinate.Longitude);
		}
	}
}
