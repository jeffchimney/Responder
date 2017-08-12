using System;
using System.Collections.Generic;
using Xamarin.Forms;
namespace Responder
{
	public interface GetLocationInterface
	{
		string GetLocation();
		List<ResponderResult> GetAllResponders();
		bool AskForLocationPermissions();
		void RegisterForPushNotifications();
		void StartMonitoringLocationInBackground();
		void StopMonitoringLocationChanges();
		void StartListening();
		void StopListening();
        //void SaveTimeToHallLocally();
        //void SaveDistanceFromHallLocally();
	}
}
