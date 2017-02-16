using System;
using Xamarin.Forms;
namespace Responder
{
	public interface GetLocationInterface
	{
		string GetLocation();
		bool AskForLocationPermissions();
		void RegisterForPushNotifications();
		void StartMonitoringLocationInBackground();
		void StopMonitoringLocationChanges();
		void StartListening();
	}
}
