using Xamarin.Forms;
using System;

namespace Responder
{
	public partial class MainTab : ContentPage
	{
		MainPage parentPage;

		Image logo = new Image
		{
			Source = "firehalllogo.png",
			Aspect = Aspect.AspectFill,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Start
		};

		StackLayout placeholder = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 100,
			VerticalOptions = LayoutOptions.CenterAndExpand
		};
		StackLayout placeholder2 = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 100,
			VerticalOptions = LayoutOptions.CenterAndExpand
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

		public static bool responding = false;
		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
        public SettingsTabInterface SettingsInterface = DependencyService.Get<SettingsTabInterface>();

		public MainTab(MainPage parent)
		{
			parentPage = parent;

            btnCallToHall.Clicked += CallToHallButtonPressed;
            btnNotResponding.Clicked += BtnNotResponding_Clicked;
			btnRespondingFirehall.Clicked += RespondingFirehallButtonPressed;

			Padding = new Thickness(10);

            bool bIsAdmin = SettingsInterface.IsAdmin();

            Content = new StackLayout
            {
                Spacing = 5,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { logo, placeholder, placeholder2, btnCallToHall, btnNotResponding, btnRespondingFirehall }
            };

            if (bIsAdmin) {
                btnCallToHall.IsVisible = true;
            } else {
                btnCallToHall.IsVisible = false;
            }

			InitializeComponent();
		}

		private void CallToHallButtonPressed(object sender, EventArgs e)
		{
            // trigger push notification
            if (btnCallToHall.BackgroundColor == Color.Orange)
            {
                btnCallToHall.BackgroundColor = Color.Gray;
                btnCallToHall.Text = "Call to Hall";
            } else {
				btnCallToHall.BackgroundColor = Color.Orange;
				btnCallToHall.Text = "Complete";
            }
		}

        void BtnNotResponding_Clicked(object sender, EventArgs e)
        {
            btnRespondingFirehall.BackgroundColor = Color.Gray;
            btnRespondingFirehall.Text = "Respond";

            LocationInterface.StopListening();
        }

        private void RespondingFirehallButtonPressed(object sender, EventArgs e)
		{
			if (!responding) // start responding
			{
				responding = true;

				btnRespondingFirehall.BackgroundColor = Color.Orange;
				btnRespondingFirehall.Text = "Stop Responding";

				var seconds = TimeSpan.FromSeconds(30);
				LocationInterface.StartListening();
				string result = LocationInterface.GetLocation();
				Device.StartTimer(seconds, () =>
				{

					//call your method to check for notifications here
					result = LocationInterface.GetLocation();

					// Returning true means you want to repeat this timer, false stops it.
					if (result.Contains("AtHall"))
					{
						btnRespondingFirehall.BackgroundColor = Color.Green;
						btnRespondingFirehall.Text = "Arrived";

						responding = false;
					}
					return responding;
				});
			}
			else // stop responding
			{
				responding = false;

				LocationInterface.StopListening();
			}
		}
	}
}
