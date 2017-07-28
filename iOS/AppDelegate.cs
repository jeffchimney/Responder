using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;

using Foundation;
using UIKit;
using WindowsAzure.Messaging; 
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace Responder.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private SBNotificationHub Hub { get; set; }
		public const string ConnectionString = "Endpoint=sb://responder.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nT+a/wIBuzMtDXWcUGKaU2yv+uO+1gHP7L0PFBZmWVw=";
		public const string NotificationHubPath = "ResponderPushHub";

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                                   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                                   new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

			global::Xamarin.Forms.Forms.Init();
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
            // Get current device token
            var DeviceToken = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(DeviceToken))
            {
            	DeviceToken = DeviceToken.Trim('<').Trim('>');
            }
            Console.Out.WriteLine(DeviceToken);
            // Get previous device token
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
            {
            	//TODO: Put your own logic here to notify your server that the device token has changed/been created!
            	Console.Out.Write("Device token has been changed/created: " + DeviceToken);
            }

            // Save new device token 
            NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");


			MobileServiceClient MobileService = new MobileServiceClient("https://responderdev.azurewebsites.net");

            Hub = new SBNotificationHub(ConnectionString, NotificationHubPath);

			// Unregister any previous instances using the device token
			Hub.UnregisterAllAsync(DeviceToken, (error) =>
			{
			    if (error != null)
			    {
                    // Error unregistering
                    Console.Out.WriteLine("Error registering");
			        return;
			    }

			    // Register this device with the notification hub
			    Hub.RegisterNativeAsync(DeviceToken, null, (registerError) =>
			    {
			        if (registerError != null)
			        {
			            // Error registering
                        Console.Out.WriteLine("Error registering");
		            }
                    Console.Out.WriteLine("Registered");
			    });
			});
		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			ProcessNotification(userInfo, false);
		}

		void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alertString = string.Empty;
				string paramString = string.Empty;

				if (aps.ContainsKey(new NSString("alert")))
					alertString = (aps[new NSString("alert")] as NSString).ToString();

				if (aps.ContainsKey(new NSString("param")))
					paramString = (aps[new NSString("param")] as NSString).ToString();

				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alertString))
					{
						UIAlertView avAlert = new UIAlertView("Awesome Notification", alertString, null,
							NSBundle.MainBundle.LocalizedString("Cancel", "Cancel"),
							NSBundle.MainBundle.LocalizedString("OK", "OK"));

						avAlert.Clicked += (sender, buttonArgs) =>
						{
							if (buttonArgs.ButtonIndex != avAlert.CancelButtonIndex)
							{
								if (!string.IsNullOrEmpty(paramString))
								{
									//App.Current.MainPage = new NavigationPage(new PushNotifMessageDisplay(paramString));
								}
							}
						};

						avAlert.Show();
					}
				}
			}
		}

		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

			string alert = string.Empty;
			if (aps.ContainsKey(new NSString("alert")))
				alert = (aps[new NSString("alert")] as NSString).ToString();

			//show alert
			if (!string.IsNullOrEmpty(alert))
			{
				UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
				avAlert.Show();
			}
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
		}
	}
}

// Endpoint=sb://responder.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nT+a/wIBuzMtDXWcUGKaU2yv+uO+1gHP7L0PFBZmWVw=
// Endpoint=sb://responder.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=DqB4V3jxfj3ftxM95oN3zWdc3U51XBdm3JTxTonOndE=