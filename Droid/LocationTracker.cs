using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
 
namespace Responder.Droid
{
	/// <summary>
	/// Class provides GPS location.
	/// Note: Requires permission ACCESS_FINE_LOCATION.
	/// Usage: 
	///   1. Create the class instance. 
	///   2. Call InitializeLocationManager(). 
	///   3. Check "CurrentLocation" when you wish or 
	///      subscribe to "LocationChanged" event to be notified.
	///   4. When you do not need it any more - call Dispose().
	/// </summary>
	public class LocationTracker : Java.Lang.Object, ILocationListener, IDisposable
	{
		/// <summary>
		/// Current location
		/// </summary>
		public Location CurrentLocation { get; set; }

		/// <summary>
		/// Current location (if available) or status message if not available
		/// </summary>
		public string CurrentLocationString { get; set; }

		public Availability CurrentGPSProviderStatus = Availability.Available;

		/// <summary>
		/// Tracker Status - true if ok, false if no GPS location
		/// </summary>
		public bool IsGettingLocation { get; set; }

		/// <summary>
		/// True if GPS provider is enabled in settings, otherwise false
		/// </summary>
		public bool IsGPSProviderEnabled { get; set; }

		// Event handlers
		public event EventHandler<Location> LocationChanged;
		public event EventHandler GPSProviderDisabled;
		public event EventHandler GPSProviderEnabled;
		public event EventHandler<Availability> GPSStatusChanged;

		public LocationManager _locationManager = null;
		Context _context;
		bool _isGiveToastsOnStatusChanges;
		bool _isFirstLocationReported = false;

		// The minimum distance to change Updates in meters
		long _minDistanceChangeForUpdatesMeters = 10; // 10 meters

		// The minimum time between updates in milliseconds
		long _minTimeBetweenUpdatesMs = 1000 * 30; // 30 seconds

		public LocationTracker(Context context, bool giveToastsOnStatusChanges = true, long minDistanceChangeForUpdatesMeters = 10, long minTimeBetweenUpdatesMs = 30000)
		{
			this._context = context;
			this._isGiveToastsOnStatusChanges = giveToastsOnStatusChanges;
			_minDistanceChangeForUpdatesMeters = minDistanceChangeForUpdatesMeters;
			_minTimeBetweenUpdatesMs = minTimeBetweenUpdatesMs;
			IsGettingLocation = false;
			IsGPSProviderEnabled = false;
		}

		/// <summary>
		/// Initialize location services and request getting location updates and events.
		/// Run this methid after construction of the class instance.
		/// Run it in the UI thread if you wish the GPS settings alert to be automatically displayed.
		/// </summary>
		public void InitializeLocationManager(bool isShowGPSSettingsAlert = true)
		{
			// Get location manager
			_locationManager = (LocationManager)_context.GetSystemService(Context.LocationService);

			// Check if GPS is enabled, If not - display alert with shortcut to GPS settings
			IsGPSProviderEnabled = _locationManager.IsProviderEnabled(LocationManager.GpsProvider);
			if (IsGPSProviderEnabled)
				CurrentGPSProviderStatus = Availability.Available;
			else
				CurrentGPSProviderStatus = Availability.OutOfService;
			if (!IsGPSProviderEnabled && isShowGPSSettingsAlert)
				ShowGPSSettingsAlert();

			// Subscibe to getting location updates with the desired treshold
			_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, _minTimeBetweenUpdatesMs, _minDistanceChangeForUpdatesMeters, this);
			CurrentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

			var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.WorldWriteable);
            localSettings.Edit()
                         .PutString("Latitude", CurrentLocation.Latitude.ToString())
			             .PutString("Longitude", CurrentLocation.Longitude.ToString())
                         .Commit();
		}

		/// <summary>
		/// Display GPS disabled alert with shortcut to GPS settings
		/// </summary>
		public void ShowGPSSettingsAlert()
		{
			// See http://stacktips.com/tutorials/xamarin/alertdialog-and-dialogfragment-example-in-xamarin-android

			AlertDialog.Builder alertDialog = new AlertDialog.Builder(_context);

			// Setting Dialog Title
			alertDialog.SetTitle("GPS settings");

			// Setting Dialog Message
			alertDialog.SetMessage("GPS is not enabled. Do you want to go to settings menu?");

			// On pressing Settings button
			alertDialog.SetPositiveButton("Settings", (senderAlert, args) => {
				Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
				_context.StartActivity(intent);
			});

			// On pressing cancel button
			alertDialog.SetNegativeButton("Cancel", (senderAlert, args) => { });

			// Showing Alert Message (note - do it only on UI thread, or use Activity.RunOnUiThread method)
			Dialog dialog = alertDialog.Create();
			dialog.Show();
		}


		/// <summary>
		/// LocationListener will call this method when updates come from LocationManager
		/// </summary>
		/// <param name="location"></param>
		public void OnLocationChanged(Location location)
		{
			if (location == null)
			{
				CurrentLocationString = "No location";
				_isFirstLocationReported = false;
				IsGettingLocation = false;
				if (_isFirstLocationReported && _isGiveToastsOnStatusChanges)
				{   // if after we had location, we lost it and got null
					Toast.MakeText(_context, String.Format("Location update: null", CurrentLocation.Latitude, CurrentLocation.Longitude), ToastLength.Short).Show();
				}
			}
			else
			{
				CurrentLocation = location;
				IsGettingLocation = true;
				CurrentLocationString = String.Format("{0}:{1}", CurrentLocation.Latitude, CurrentLocation.Longitude);
				if (!_isFirstLocationReported && _isGiveToastsOnStatusChanges)
				{
					Toast.MakeText(_context, String.Format("Location update: {0}:{1}", CurrentLocation.Latitude, CurrentLocation.Longitude), ToastLength.Short).Show();
					_isFirstLocationReported = true;
				}

				var localSettings = Application.Context.GetSharedPreferences("Defaults", FileCreationMode.WorldWriteable);
				localSettings.Edit()
						 .PutString("Latitude", CurrentLocation.Latitude.ToString())
						 .PutString("Longitude", CurrentLocation.Longitude.ToString())
						 .Commit();
			}
			// Fire event
			if (LocationChanged != null)
				LocationChanged(this, location);
		}

		public void OnProviderDisabled(string provider)
		{
			if (provider == LocationManager.GpsProvider)
			{
				if (_isGiveToastsOnStatusChanges)
					Toast.MakeText(_context, "GPS provider disabled.", ToastLength.Short).Show();
				CurrentLocationString = "GPS provider disabled.";
				IsGettingLocation = false;
				_isFirstLocationReported = false;
				IsGPSProviderEnabled = false;
				// Fire event
				if (GPSProviderDisabled != null)
					GPSProviderDisabled(this, null);
			}
		}

		public void OnProviderEnabled(string provider)
		{
			if (provider == LocationManager.GpsProvider)
			{
				if (_isGiveToastsOnStatusChanges)
					Toast.MakeText(_context, "GPS provider enabled", ToastLength.Short).Show();
				CurrentLocationString = "GPS provider enabled.";
				_isFirstLocationReported = false;
				IsGPSProviderEnabled = false;
				// Fire event
				if (GPSProviderEnabled != null)
					GPSProviderEnabled(this, null);
			}
		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
			if (provider == LocationManager.GpsProvider)
			{
				if (CurrentGPSProviderStatus != status)
				{
					if (_isGiveToastsOnStatusChanges)
						Toast.MakeText(_context, "GPS Status changed: " + status.ToString(), ToastLength.Short).Show();
					CurrentLocationString = "GPS Status " + status.ToString();
					CurrentGPSProviderStatus = status;
					if (status != Availability.Available)
					{
						IsGettingLocation = false;
						_isFirstLocationReported = false;
					}
					// Fire event
					if (GPSStatusChanged != null)
						GPSStatusChanged(this, status);
				}
			}
		}


		/// <summary>
		/// Stop location services and unsubscribe from getting events
		/// </summary>
		public void StopLocationManager()
		{
			if (_locationManager != null)
			{
				_locationManager.RemoveUpdates(this);
			}
		}

		public new void Dispose()
		{
			StopLocationManager();
		}
	}
}