using Xamarin.Forms;
using System;

namespace Responder
{
	public partial class MainTab : ContentPage
	{
		MainPage parentPage;

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
			HeightRequest = 110,
			TextColor = Color.Black
		};
		Button btnRespondingScene = new Button
		{
			Text = "Responding to Scene",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 110,
			TextColor = Color.Black
		};
		Button btnOnScene = new Button
		{
			Text = "On scene",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			HeightRequest = 110,
			TextColor = Color.Black
		};
		Button btnUnavailable = new Button
		{
			Text = "Unavailable",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			WidthRequest = 165,
			HeightRequest = 110,
			TextColor = Color.Black
		};
		Button btnStandDown = new Button
		{
			Text = "Stand down",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			WidthRequest = 165,
			HeightRequest = 110,
			TextColor = Color.Black
		};
		Label lblCoords = new Label
		{
			Text = "",
			FontSize = 20,
			WidthRequest = 165,
			HeightRequest = 110,
			TextColor = Color.Black
		};

		public static bool responding = false;

		public MainTab(MainPage parent)
		{
			parentPage = parent;

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
			responding = true;

			btnRespondingFirehall.BackgroundColor = Color.Green;
			btnRespondingScene.BackgroundColor = Color.Gray;
			btnOnScene.BackgroundColor = Color.Gray;
			btnUnavailable.BackgroundColor = Color.Gray;
			btnStandDown.BackgroundColor = Color.Gray;

			var seconds = TimeSpan.FromSeconds(30);
			DependencyService.Get<GetLocationInterface>().StartListening();
			string result = DependencyService.Get<GetLocationInterface>().GetLocation();
			Device.StartTimer(seconds, () =>
			{

				//call your method to check for notifications here
				result = DependencyService.Get<GetLocationInterface>().GetLocation();
				Console.WriteLine(result.Substring(0, 4));

				// Returning true means you want to repeat this timer, false stops it.
				if (result.Substring(0, 6).Contains("DONE"))
				{
					btnRespondingFirehall.BackgroundColor = Color.Gray;
					btnRespondingFirehall.Text = "Arrived";
					responding = false;
					return false;
				}
				else
				{
					return true;
				}
			});
		}

		public void RespondingSceneButtonPressed(object sender, EventArgs e)
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
