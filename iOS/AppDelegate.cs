﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;

using Foundation;
using UIKit;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.CognitoIdentity;

namespace Responder.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private AmazonSimpleNotificationServiceClient snsClient;

		public override bool FinishedLaunching(UIApplication app, NSDictionary launchOptions)
		{
			//if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			//{
			//	var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
			//		   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
			//		   new NSSet());

			//	UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
			//	UIApplication.SharedApplication.RegisterForRemoteNotifications();
			//}
			//else
			//{
			//	UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
			//	UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
			//}

			//CognitoAWSCredentials credentials = new CognitoAWSCredentials(
			//	"us-west-2:ec8de114-9ca5-4e6a-9c84-a9e484975d0a", // Identity pool ID
			//	RegionEndpoint.USWest2 // Region
			//);



   //         snsClient = new AmazonSimpleNotificationServiceClient("AKIAJG5P2JQN2CRRM2IQ", "6mdlnDzPFC3wry1K78eC+9Gz15FnWDGFeO2tRFwt", RegionEndpoint.USWest2);

			//var loggingConfig = AWSConfigs.LoggingConfig;
			//loggingConfig.LogMetrics = true;
			//loggingConfig.LogResponses = ResponseLoggingOption.Always;
			//loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
			//loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;

            //AWSConfigs.AWSRegion = "us-west-2";

            //AWSConfigs.CorrectForClockSkew = true;
            //var offset = AWSConfigs.ClockOffset;

			global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

			return base.FinishedLaunching(app, launchOptions);
		}

        public void RegisterForNotifications() {
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

            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                "us-west-2:ec8de114-9ca5-4e6a-9c84-a9e484975d0a", // Identity pool ID
                RegionEndpoint.USWest2 // Region
            );



            snsClient = new AmazonSimpleNotificationServiceClient("AKIAJG5P2JQN2CRRM2IQ", "6mdlnDzPFC3wry1K78eC+9Gz15FnWDGFeO2tRFwt", RegionEndpoint.USWest2);

            var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
            loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;

            AWSConfigs.AWSRegion = "us-west-2";

            AWSConfigs.CorrectForClockSkew = true;
            var offset = AWSConfigs.ClockOffset;
        }

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
            var defaults = NSUserDefaults.StandardUserDefaults;

            var endpoint = defaults.StringForKey("EndpointARN");

            if (endpoint == "" || endpoint == null)
            {
                var token = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
                Console.WriteLine(token);
                if (!string.IsNullOrEmpty(token))
                {
                    string sFireHallID = defaults.StringForKey("FireHallID");
                    string sUserID = defaults.StringForKey("UserID");

                    var phoneName = sFireHallID + sUserID;

                    //register with SNS to create an endpoint ARN
                    var response = snsClient.CreatePlatformEndpointAsync(
                    new Amazon.SimpleNotificationService.Model.CreatePlatformEndpointRequest
                    {
                        Token = token,
                        CustomUserData = phoneName,
                        PlatformApplicationArn = "arn:aws:sns:us-west-2:527503918783:app/APNS/FirehallResponderProd" /* insert your platform application ARN here */
                    });

                    response.Wait();

                    subscribe(response.Result.EndpointArn);
                }
            }
		}

        public void PublishNotificationWithMessage(string sTitle, string sMessage) {
            var published = snsClient.PublishAsync("arn:aws:sns:us-west-2:527503918783:CallToHall", sMessage, sTitle);
            published.Wait();
            var test = published.Result;
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

				string alert = string.Empty;

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying
				// "  aps:{alert:"alert msg here"}  ", this will work fine.
				// But if you're using a complex alert with Localization keys, etc.,
				// your "alert" object from the aps dictionary will be another NSDictionary.
				// Basically the JSON gets dumped right into a NSDictionary,
				// so keep that in mind.
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps[new NSString("alert")] as NSString).ToString();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						avAlert.Show();
					}
				}
			}
		}

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            new UIAlertView("Error registering push notifications", error.LocalizedDescription, null, "OK", null).Show();
        }

        public void subscribe(string sEndpointARN)
        {
            // Creating the topic request and the topic and response  
            var topics = snsClient.FindTopicAsync("CallToHall");
            topics.Wait();
            var topicARN = topics.Result.TopicArn;

            // Subscribe to the endpoint of the topic  
            var subscribeRequest = new SubscribeRequest()
            {
                TopicArn = topicARN,
                Protocol = "application",
                Endpoint = sEndpointARN
            };

            var res = snsClient.SubscribeAsync(subscribeRequest);
            res.Wait();
            var results = res.Result;

            var defaults = NSUserDefaults.StandardUserDefaults;
            defaults.SetString(sEndpointARN, "EndpointARN");
        }
	}
}