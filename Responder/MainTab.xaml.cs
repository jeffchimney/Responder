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

		StackLayout test = new StackLayout
		{
			HorizontalOptions = LayoutOptions.Center,
			HeightRequest = 100,
			VerticalOptions = LayoutOptions.CenterAndExpand
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

		public MainTab(MainPage parent)
		{
			parentPage = parent;

			btnRespondingFirehall.Clicked += RespondingFirehallButtonPressed;

			Padding = new Thickness(10);

			Content = new StackLayout
			{
				Spacing = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { logo, test, btnRespondingFirehall }
			};

			InitializeComponent();
		}

		private void RespondingFirehallButtonPressed(object sender, EventArgs e)
		{
			if (!responding) // start responding
			{
				responding = true;

				btnRespondingFirehall.BackgroundColor = Color.Orange;
				btnRespondingFirehall.Text = "Stop Responding";
				//btnRespondingScene.BackgroundColor = Color.Gray;
				//btnOnScene.BackgroundColor = Color.Gray;
				//btnUnavailable.BackgroundColor = Color.Gray;
				//btnStandDown.BackgroundColor = Color.Gray;

				var seconds = TimeSpan.FromSeconds(30);
				LocationInterface.StartListening();
				string result = LocationInterface.GetLocation();
				Device.StartTimer(seconds, () =>
				{

					//call your method to check for notifications here
					result = LocationInterface.GetLocation();

					// Returning true means you want to repeat this timer, false stops it.
					if (result.Contains("DONE"))
					{
						btnRespondingFirehall.BackgroundColor = Color.Gray;
						btnRespondingFirehall.Text = "Arrived";
						responding = false;
					}
					return responding;
				});
			}
			else // stop responding
			{
				responding = false;

				btnRespondingFirehall.BackgroundColor = Color.Gray;
				btnRespondingFirehall.Text = "Respond";
				//btnRespondingScene.BackgroundColor = Color.Gray;
				//btnOnScene.BackgroundColor = Color.Gray;
				//btnUnavailable.BackgroundColor = Color.Gray;
				//btnStandDown.BackgroundColor = Color.Gray;
			}
		}

		//public void RespondingSceneButtonPressed(object sender, EventArgs e)
		//{
		//	btnRespondingFirehall.BackgroundColor = Color.Gray;
		//	//btnRespondingScene.BackgroundColor = Color.Green;
		//	//btnOnScene.BackgroundColor = Color.Gray;
		//	btnUnavailable.BackgroundColor = Color.Gray;
		//	//btnStandDown.BackgroundColor = Color.Gray;
		//}

		//private void OnSceneButtonPressed(object sender, EventArgs e)
		//{
		//	btnRespondingFirehall.BackgroundColor = Color.Gray;
		//	btnRespondingScene.BackgroundColor = Color.Gray;
		//	btnOnScene.BackgroundColor = Color.Green;
		//	btnUnavailable.BackgroundColor = Color.Gray;
		//	btnStandDown.BackgroundColor = Color.Gray;
		//}

		//private void UnavailableButtonPressed(object sender, EventArgs e)
		//{
		//	btnRespondingFirehall.BackgroundColor = Color.Gray;
		//	//btnRespondingScene.BackgroundColor = Color.Gray;
		//	//btnOnScene.BackgroundColor = Color.Gray;
		//	//btnUnavailable.BackgroundColor = Color.Red;
		//	//btnStandDown.BackgroundColor = Color.Gray;
		//}

		//private void StandDownButtonPressed(object sender, EventArgs e)
		//{
		//	btnRespondingFirehall.BackgroundColor = Color.Gray;
		//	btnRespondingScene.BackgroundColor = Color.Gray;
		//	btnOnScene.BackgroundColor = Color.Gray;
		//	btnUnavailable.BackgroundColor = Color.Gray;
		//	btnStandDown.BackgroundColor = Color.Green;
		//}
	}
}
