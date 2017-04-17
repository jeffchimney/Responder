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
		//Button btnRespondingScene = new Button
		//{
		//	Text = "Responding to Scene",
		//	BackgroundColor = Color.Gray,
		//	FontSize = 20,
		//	HeightRequest = 110,
		//	TextColor = Color.Black
		//};
		//Button btnOnScene = new Button
		//{
		//	Text = "On scene",
		//	BackgroundColor = Color.Gray,
		//	FontSize = 20,
		//	HeightRequest = 110,
		//	TextColor = Color.Black
		//};
		//Button btnUnavailable = new Button
		//{
		//	Text = "Unavailable",
		//	BackgroundColor = Color.Gray,
		//	FontSize = 20,
		//	HeightRequest = 75,
		//	TextColor = Color.Black,
		//	HorizontalOptions = LayoutOptions.FillAndExpand
		//};
		//Button btnStandDown = new Button
		//{
		//	Text = "Stand down",
		//	BackgroundColor = Color.Gray,
		//	FontSize = 20,
		//	WidthRequest = 165,
		//	HeightRequest = 110,
		//	TextColor = Color.Black
		//};
		//Label lblCoords = new Label
		//{
		//	Text = "",
		//	FontSize = 20,
		//	WidthRequest = 165,
		//	HeightRequest = 110,
		//	TextColor = Color.Black
		//};

		public static bool responding = false;

		public MainTab(MainPage parent)
		{
			parentPage = parent;

			//set up touch delegates
			btnRespondingFirehall.Clicked += RespondingFirehallButtonPressed;
			//btnRespondingScene.Clicked += RespondingSceneButtonPressed;
			//btnOnScene.Clicked += OnSceneButtonPressed;
			//btnUnavailable.Clicked += UnavailableButtonPressed;
			//btnStandDown.Clicked += StandDownButtonPressed;

			Padding = new Thickness(20);

			//StackLayout title = new StackLayout
			//{
			//	Children = {
			//		lblTitle
			//	},
			//	Orientation = StackOrientation.Horizontal,
			//	HorizontalOptions = LayoutOptions.FillAndExpand,
			//};

			Content = new StackLayout
			{
				Spacing = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { logo, test, btnRespondingFirehall } //btnRespondingScene,btnOnScene,sideBySide,lblCoords, btnUnavailable
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
				DependencyService.Get<GetLocationInterface>().StartListening();
				string result = DependencyService.Get<GetLocationInterface>().GetLocation();
				Device.StartTimer(seconds, () =>
				{

					//call your method to check for notifications here
					result = DependencyService.Get<GetLocationInterface>().GetLocation();

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
