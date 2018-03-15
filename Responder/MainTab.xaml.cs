using Xamarin.Forms;
using System;
using System.Threading;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.CognitoIdentity;

namespace Responder
{
	public partial class MainTab : ContentPage
	{
		MainPage parentPage;

		Image logo = new Image
		{
			Source = "firehalllogo2.png",
			Aspect = Aspect.AspectFill,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Start
		};

		StackLayout placeholder = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 75,
			VerticalOptions = LayoutOptions.CenterAndExpand
		};
        ActivityIndicator activityIndicator = new ActivityIndicator
        {
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 75,
			VerticalOptions = LayoutOptions.CenterAndExpand
        };
		//StackLayout placeholder2 = new StackLayout
		//{
		//	HorizontalOptions = LayoutOptions.Center,
		//	HeightRequest = 75,
		//	VerticalOptions = LayoutOptions.CenterAndExpand
		//};
		StackLayout placeholder3 = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 75,
			VerticalOptions = LayoutOptions.CenterAndExpand
		};
		StackLayout placeholder4 = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 75,
			VerticalOptions = LayoutOptions.CenterAndExpand
		};

        Label lblStatus = new Label
        {
            Text = "Idle",
			FontSize = 20,
			HeightRequest = 75,
			TextColor = Color.Black,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Center
        };

		Button btnCallToHall = new Button
		{
			Text = "Call to Hall",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 75,
			TextColor = Color.Black,
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions = LayoutOptions.EndAndExpand
		};

		Button btnNotResponding = new Button
		{
			Text = "Not Responding",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 75,
			TextColor = Color.Black,
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions = LayoutOptions.EndAndExpand
		};

		Button btnRespondingFirehall = new Button
		{
			Text = "Respond",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 75,
			TextColor = Color.Black,
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions = LayoutOptions.EndAndExpand
		};

        Button btnCancel = new Button
        {
            Text = "Cancel",
            BackgroundColor = Color.Orange,
            FontSize = 20,
            HeightRequest = 60,
            WidthRequest = 110,
            TextColor = Color.Black,
            HorizontalOptions = LayoutOptions.EndAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        public class TimerState
        {
            public int counter = 0;
            public Timer tmr;
        }

		public static bool responding = false;
        public static bool timerAlreadyStarted = false;
		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
        public SettingsTabInterface SettingsInterface = DependencyService.Get<SettingsTabInterface>();
        public TimerState s = new TimerState();


		public MainTab(MainPage parent)
		{
			parentPage = parent;

            btnCallToHall.Clicked += CallToHallButtonPressed;
            btnNotResponding.Clicked += BtnNotResponding_Clicked;
			btnRespondingFirehall.Clicked += RespondingFirehallButtonPressed;
            btnCancel.Clicked += BtnCancel_Pressed;

			Padding = new Thickness(25);

            Content = new StackLayout
            {
                Spacing = 5,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { logo, placeholder, lblStatus, activityIndicator, btnCallToHall, btnNotResponding, btnRespondingFirehall }
            };

            bool btnCallToHallVisibility = SettingsInterface.IsAdmin();

            btnCallToHall.IsVisible = btnCallToHallVisibility;

			InitializeComponent();

            LocationInterface.AskForLocationPermissions();
		}

		private async void CallToHallButtonPressed(object sender, EventArgs e)
		{
            // trigger push notification
            bool bSendNotification = await DisplayAlert("Are you sure?", "", "Send", "Cancel");

            if (bSendNotification)
            {
                LocationInterface.CallToHall("FireHall Alert", "FireHall Incident - Please Respond");
                //PublishNotificationWithMessage("FireHall Alert", "FireHall Incident - Please Respond");
                lblStatus.Text = "Call to Hall Sent";
            } else {
                lblStatus.Text = "Call to Hall Cancelled";
            }
		}

        private void PublishNotificationWithMessage(string sTitle, string sMessage)
        {
            var credentials = new CognitoAWSCredentials(
            "us-west-2:ec8de114-9ca5-4e6a-9c84-a9e484975d0a", // Identity pool ID
            RegionEndpoint.USWest2 // Region
            );

            var snsClient = new AmazonSimpleNotificationServiceClient("AKIAJG5P2JQN2CRRM2IQ", "6mdlnDzPFC3wry1K78eC+9Gz15FnWDGFeO2tRFwt", RegionEndpoint.USWest2);

            var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
            loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;

            AWSConfigs.AWSRegion = "us-west-2";

            AWSConfigs.CorrectForClockSkew = true;
            var offset = AWSConfigs.ClockOffset;


            var published = snsClient.PublishAsync("arn:aws:sns:us-west-2:527503918783:CallToHall", sMessage, sTitle);
            published.Wait();
            var test = published.Result;
        }

        void BtnNotResponding_Clicked(object sender, EventArgs e)
        {
            // dispose of preexisting timer if there is one (there shouldnt be)
            if (s.tmr != null)
            {
                s.tmr.Dispose();
                s.tmr = null;
            }
            
            responding = false;
            activityIndicator.IsRunning = true;

            if (LocationInterface.HasNetworkConnectivity())
            {
                LocationInterface.StopListening();
            }

            activityIndicator.IsRunning = false;
            lblStatus.Text = "Not Responding";
            ShowCancelButtonHideOthers();
        }

        private async void RespondingFirehallButtonPressed(object sender, EventArgs e)
        {
			responding = true;
            ShowCancelButtonHideOthers();
            activityIndicator.IsRunning = true;

			lblStatus.Text = "Responding";

            if (LocationInterface.HasNetworkConnectivity())
            {
                LocationInterface.StartListening();

                string result = LocationInterface.GetLocation();
                if (!result.Contains("Location Services Not Enabled"))
                {
                    // dispose of preexisting timer if there is one (there shouldnt be)
                    if (s.tmr != null)
                    {
                        s.tmr.Dispose();
                        s.tmr = null;
                    }

                    // instantiate new timer
                    TimerCallback timerDelegate = new TimerCallback(CheckStatus);
                    Timer timer = new Timer(timerDelegate, s, 10000, 10000);
                    s.tmr = timer;
                }
                else
                { // show alert saying user doesn't have location services enabled.
                    responding = false;
                    timerAlreadyStarted = false;
                    btnRespondingFirehall.BackgroundColor = Color.Gray;
                    lblStatus.Text = "";
                    var accepted = await DisplayAlert("Enable Location Services", "Responder needs access to your location in order to track your progress to the hall.", "Settings", "Cancel");

                    if (accepted)
                    {
                        LocationInterface.LinkToSettings();
                    }
                }
            }

            activityIndicator.IsRunning = false;
		}

        public void CheckStatus(Object state)
        {
            TimerState timerState = (TimerState)state;
            if (responding)
            {
                string result = LocationInterface.GetLocation();

                // Returning true means you want to repeat this timer, false stops it.
                if (result.Contains("AtHall"))
                {
                    btnRespondingFirehall.BackgroundColor = Color.Green;
                    lblStatus.Text = "Arrived";

                    responding = false;
                    timerState.tmr.Dispose();
                    timerState.tmr = null;
                }
            }
        }

        private void BtnCancel_Pressed(object sender, EventArgs e)
        {
            HideCancelButtonShowOthers();
            if (responding && LocationInterface.HasNetworkConnectivity())
            {
                lblStatus.Text = "Stopped Responding";
                LocationInterface.StopListening();
            } else
            {
                lblStatus.Text = "Idle";
            }
            responding = false;

            // dispose of preexisting timer if there is one (there shouldnt be)
            if (s.tmr != null) {
                s.tmr.Dispose();
                s.tmr = null;
            }
		}

		private void SetTimeToHallLocally()
		{

		}

        private void SetDistanceToHallLocally()
        {
            
        }

        public void SetCallToHallVisibility(bool bIsAdmin) {
           btnCallToHall.IsVisible = bIsAdmin;
        }

        public void ShowCancelButtonHideOthers() {
			Content = new StackLayout
			{
				Spacing = 5,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { logo, placeholder, lblStatus, activityIndicator, placeholder3, placeholder4, btnCancel }
			};
        }

        public void HideCancelButtonShowOthers() {
			Content = new StackLayout
			{
				Spacing = 5,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { logo, placeholder, lblStatus, activityIndicator, btnCallToHall, btnNotResponding, btnRespondingFirehall }
			};
        }
	}
}
