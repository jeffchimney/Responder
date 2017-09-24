using Xamarin.Forms;
using System;

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
		StackLayout placeholder2 = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 75,
			VerticalOptions = LayoutOptions.CenterAndExpand
		};
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

		public static bool responding = false;
        public static bool timerAlreadyStarted = false;
		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
        public SettingsTabInterface SettingsInterface = DependencyService.Get<SettingsTabInterface>();

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
                Children = { logo, placeholder, lblStatus, placeholder2, btnCallToHall, btnNotResponding, btnRespondingFirehall }
            };

            bool btnCallToHallVisibility = SettingsInterface.IsAdmin();

            btnCallToHall.IsVisible = btnCallToHallVisibility;

			InitializeComponent();
		}

		private async void CallToHallButtonPressed(object sender, EventArgs e)
		{
            // trigger push notification
            bool bSendNotification = await DisplayAlert("Are you sure you want to send?", "", "Send", "Cancel");

            if (bSendNotification)
            {
                LocationInterface.CallToHall("Get your ass to the hall!", "There has been a terrible accident and grandma needs her cat out of the toilet.");
                lblStatus.Text = "Call to Hall Sent";
            } else {
                lblStatus.Text = "Call to Hall Cancelled";
            }
		}

        void BtnNotResponding_Clicked(object sender, EventArgs e)
        {
            responding = false;

            LocationInterface.StopListening();

            lblStatus.Text = "Not Responding";
            ShowCancelButtonHideOthers();
        }

        private async void RespondingFirehallButtonPressed(object sender, EventArgs e)
		{
			responding = true;
            ShowCancelButtonHideOthers();

			lblStatus.Text = "Responding";

			var seconds = TimeSpan.FromSeconds(15);
            if (LocationInterface.HasNetworkConnectivity())
            {
                LocationInterface.StartListening();
            }
            if (!timerAlreadyStarted)
            {
                string result = LocationInterface.GetLocation();
                if (!result.Contains("Location Services Not Enabled"))
                {
                    timerAlreadyStarted = true;
                    Device.StartTimer(seconds, () =>
                    {
                        if (responding)
                        {
                            timerAlreadyStarted = true;
                            //call your method to check for notifications here
                            result = LocationInterface.GetLocation();

                            // Returning true means you want to repeat this timer, false stops it.
                            if (result.Contains("AtHall"))
                            {
                                btnRespondingFirehall.BackgroundColor = Color.Green;
                                lblStatus.Text = "Arrived";

                                responding = false;
                                timerAlreadyStarted = false;
                            }
                        }
                        return responding;
                    });
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
		}

        private void BtnCancel_Pressed(object sender, EventArgs e)
        {
            HideCancelButtonShowOthers();
            responding = false;
            if (responding)
            {
                lblStatus.Text = "Stopped Responding";
                LocationInterface.StopListening();
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
				Children = { logo, placeholder, lblStatus, placeholder2, placeholder3, placeholder4, btnCancel }
			};
        }

        public void HideCancelButtonShowOthers() {
			Content = new StackLayout
			{
				Spacing = 5,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { logo, placeholder, lblStatus, placeholder2, btnCallToHall, btnNotResponding, btnRespondingFirehall }
			};
        }
	}
}
