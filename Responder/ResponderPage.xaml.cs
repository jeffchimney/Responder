using Xamarin.Forms;
using System;
using Plugin.Geolocator;

namespace Responder
{
	public partial class ResponderPage : ContentPage
	{
		Label lblTitle = new Label
		{
			Text = "FH Responder",
			FontSize = 40,
			HorizontalTextAlignment = TextAlignment.Center
		};
		Button btnRespondingFirehall = new Button
		{
			Text = "Responding to firehall",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 120,
			TextColor = Color.Black
		};
		Button btnRespondingScene = new Button
		{
			Text = "Responding to Scene",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 120,
			TextColor = Color.Black
		};
		Button btnOnScene = new Button
		{
			Text = "On scene",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 120,
			TextColor = Color.Black
		};
		Button btnUnavailable = new Button
		{
			Text = "Unavailable",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			WidthRequest = 165,
			HeightRequest = 120,
			TextColor = Color.Black
		};
		Button btnStandDown = new Button
		{
			Text = "Stand down",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			WidthRequest = 165,
			HeightRequest = 120,
			TextColor = Color.Black
		};
		Label lblCoords = new Label
		{
			Text = "",
			FontSize = 20,
			WidthRequest = 165,
			HeightRequest = 120,
			TextColor = Color.Black
		};

		public ResponderPage()
		{
			//set up touch delegates
			btnRespondingFirehall.Clicked += RespondingFirehallButtonPressed;
			btnRespondingScene.Clicked += RespondingSceneButtonPressed;
			btnOnScene.Clicked += OnSceneButtonPressed;
			btnUnavailable.Clicked += UnavailableButtonPressed;
			btnStandDown.Clicked += StandDownButtonPressed;

			Padding = new Thickness(20);

			StackLayout sideBySide = new StackLayout
			{
				Children = {
					btnUnavailable,
					btnStandDown
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			Content = new StackLayout
			{
				Spacing = 10,
				Children = { lblTitle, btnRespondingFirehall, btnRespondingScene, btnOnScene, sideBySide, lblCoords }
			};

			InitializeComponent();
		}

		private void RespondingFirehallButtonPressed(object sender, EventArgs e)
		{
			btnRespondingFirehall.BackgroundColor = Color.Green;
			btnRespondingScene.BackgroundColor = Color.Gray;
			btnOnScene.BackgroundColor = Color.Gray;
			btnUnavailable.BackgroundColor = Color.Gray;
			btnStandDown.BackgroundColor = Color.Gray;

			var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;

			try
			{
				//locator.StartListeningAsync(100,20.0, false);
				var position = locator.GetPositionAsync();
				//locator.StopListeningAsync();

				lblCoords.Text = position.Result.Latitude + ", " + position.Result.Longitude;
			}
			catch (Exception ex)
			{
				lblCoords.Text = "Couldn't get coordinates";
			}
		}

		private void RespondingSceneButtonPressed(object sender, EventArgs e)
		{
			btnRespondingFirehall.BackgroundColor = Color.Gray;
			btnRespondingScene.BackgroundColor = Color.Green;
			btnOnScene.BackgroundColor = Color.Gray;
			btnUnavailable.BackgroundColor = Color.Gray;
			btnStandDown.BackgroundColor = Color.Gray;
		}

		private void OnSceneButtonPressed(object sender, EventArgs e)
		{
			btnRespondingFirehall.BackgroundColor = Color.Gray;
			btnRespondingScene.BackgroundColor = Color.Gray;
			btnOnScene.BackgroundColor = Color.Green;
			btnUnavailable.BackgroundColor = Color.Gray;
			btnStandDown.BackgroundColor = Color.Gray;
		}

		private void UnavailableButtonPressed(object sender, EventArgs e)
		{
			btnRespondingFirehall.BackgroundColor = Color.Gray;
			btnRespondingScene.BackgroundColor = Color.Gray;
			btnOnScene.BackgroundColor = Color.Gray;
			btnUnavailable.BackgroundColor = Color.Green;
			btnStandDown.BackgroundColor = Color.Gray;
		}

		private void StandDownButtonPressed(object sender, EventArgs e)
		{
			btnRespondingFirehall.BackgroundColor = Color.Gray;
			btnRespondingScene.BackgroundColor = Color.Gray;
			btnOnScene.BackgroundColor = Color.Gray;
			btnUnavailable.BackgroundColor = Color.Gray;
			btnStandDown.BackgroundColor = Color.Green;
		}
	}
}
