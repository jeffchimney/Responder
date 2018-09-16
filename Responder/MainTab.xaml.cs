using Xamarin.Forms;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json.Linq;
using System.IO;

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

        //public class TimerState
        //{
        //    public int counter = 0;
        //    public Timer tmr;
        //}

		public static bool responding = false;
        public static bool timerAlreadyStarted = false;
		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
        public SettingsTabInterface SettingsInterface = DependencyService.Get<SettingsTabInterface>();
        //public TimerState s = new TimerState();

        private const string URL = "https://firehall-fn.azurewebsites.net/api/SendPush";
        private static readonly HttpClient client = new HttpClient();

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
            if (LocationInterface.HasNetworkConnectivity())
            {
                // trigger push notification
                bool bSendNotification = await DisplayAlert("Are you sure?", "", "Send", "Cancel");

                if (bSendNotification)
                {
                    var sData = "FireHall Incident - Please Respond";
                    GetPOSTResponse(URL, sData);
                    LocationInterface.CallToHall("FireHall Alert", "FireHall Incident - Please Respond");
                    //PublishNotificationWithMessage("FireHall Alert", "FireHall Incident - Please Respond");
                    lblStatus.Text = "Call to Hall Sent";
                }
                else
                {
                    lblStatus.Text = "Call to Hall Cancelled";
                }
            } else {
                lblStatus.Text = "Check Internet Connection";
            }
		}

        private void GetPOSTResponse(string sUri, string data)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sUri);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            string result = "";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{ \"message\": \"" + data +  "\" }";
                Console.Out.WriteLine(json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebRequest.HaveResponse && response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();
                            Console.Out.Write(result);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)e.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string error = reader.ReadToEnd();
                            result = error;
                            Console.Out.Write(result);
                        }
                    }

                }
            }

            //var responseString = await sUri
            //    .PostUrlEncodedAsync(new { }) // message = data
            //    .ReceiveString();

            //Console.Out.WriteLine(responseString);
        }

        void BtnNotResponding_Clicked(object sender, EventArgs e)
        {
            if (LocationInterface.HasNetworkConnectivity())
            {
                responding = false;
                LocationInterface.StopListening();
                lblStatus.Text = "Not Responding";
                ShowCancelButtonHideOthers();
            } else {
                lblStatus.Text = "Check Internet Connection";
            }
        }

        private async void RespondingFirehallButtonPressed(object sender, EventArgs e)
        {
            if (LocationInterface.HasNetworkConnectivity())
            {
                if (LocationInterface.CheckAuthorizationStatus()) {
                    responding = true;
                    LocationInterface.StartListening();
                    LocationInterface.StartMonitoringLocationInBackground();

                    responding = true;
                    LocationInterface.SetResponding(true);
                    ShowCancelButtonHideOthers();
                    activityIndicator.IsRunning = true;

                    lblStatus.Text = "Responding";
                } else {
                    // show alert saying user doesn't have location services enabled.
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

                //string result = LocationInterface.GetLocation();
                //if (!result.Contains("Location Services Not Enabled"))
                //{
                //    // dispose of preexisting timer if there is one (there shouldnt be)
                //    if (s.tmr != null)
                //    {
                //        s.tmr.Dispose();
                //        s.tmr = null;
                //    }

                //    // instantiate new timer
                //    TimerCallback timerDelegate = new TimerCallback(CheckStatus);
                //    Timer timer = new Timer(timerDelegate, s, 10000, 10000);
                //    s.tmr = timer;
                //}
                //else
                //{ // show alert saying user doesn't have location services enabled.
                //    responding = false;
                //    timerAlreadyStarted = false;
                //    btnRespondingFirehall.BackgroundColor = Color.Gray;
                //    lblStatus.Text = "";
                //    var accepted = await DisplayAlert("Enable Location Services", "Responder needs access to your location in order to track your progress to the hall.", "Settings", "Cancel");

                //    if (accepted)
                //    {
                //        LocationInterface.LinkToSettings();
                //    }
                //}
            } else {
                lblStatus.Text = "Check Internet Connection";
            }

            activityIndicator.IsRunning = false;
		}

        //public void CheckStatus(Object state)
        //{
        //    TimerState timerState = (TimerState)state;
        //    if (responding)
        //    {
        //        string result = LocationInterface.GetLocation();

        //        // Returning true means you want to repeat this timer, false stops it.
        //        if (result.Contains("AtHall"))
        //        {
        //            btnRespondingFirehall.BackgroundColor = Color.Green;
        //            lblStatus.Text = "Arrived";

        //            responding = false;
        //            timerState.tmr.Dispose();
        //            timerState.tmr = null;
        //        }
        //    }
        //}

        private void BtnCancel_Pressed(object sender, EventArgs e)
        {
            if (LocationInterface.HasNetworkConnectivity())
            {
                HideCancelButtonShowOthers();
                lblStatus.Text = "Stopped Responding";
                LocationInterface.SetResponding(false);
                LocationInterface.StopListening();
                responding = false;
            } else
            {
                lblStatus.Text = "Check Internet Connection";
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
