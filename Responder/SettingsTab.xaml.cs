using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class SettingsTab : ContentPage
	{

		public MainPage parentPage;

		Image logo = new Image()
		{
			Source = "firehalllogo.png",
			Aspect = Aspect.AspectFill,
			HorizontalOptions = LayoutOptions.Center,
			VerticalOptions = LayoutOptions.Start
		};

		Label lblActivation = new Label
		{
			Text = "Activation",
			FontSize = 15,
			HorizontalTextAlignment = TextAlignment.Center
		};

		Label lblHallID = new Label
		{
			Text = "Firehall ID:",
			FontSize = 15,
			HorizontalTextAlignment = TextAlignment.Start,
			HorizontalOptions = LayoutOptions.Center
		};

		// Hall id is 6 characters long
		Entry txtHallID1 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID2 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID3 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID4 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID5 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID6 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};

		Label lblUserID = new Label
		{
			Text = "User ID:",
			FontSize = 15,
			HorizontalTextAlignment = TextAlignment.Start,
			HorizontalOptions = LayoutOptions.Center
		};
		// User id is 2 characters long
		Entry txtUserID1 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtUserID2 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 45,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};

		Button btnSubmit = new Button
		{
			Text = "Submit",
			BackgroundColor = Color.Gray,
			FontSize = 20,
			WidthRequest = 10,
			TextColor = Color.Black,
			VerticalOptions = LayoutOptions.Start
		};

		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
		public SettingsTabInterface SettingsInterface = DependencyService.Get<SettingsTabInterface>();

		public SettingsTab(MainPage parent)
		{
			parentPage = parent;
			//set up touch delegates
			btnSubmit.Clicked += SubmitButtonPressed;

			Padding = new Thickness(25);

			StackLayout accountID = new StackLayout
			{
				Spacing = 5,
				Children = { txtHallID1, txtHallID2, txtHallID3, txtHallID4, txtHallID5, txtHallID6 },
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			StackLayout userID = new StackLayout
			{
				Spacing = 5,
				Children = { txtUserID1, txtUserID2 },
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			Content = new StackLayout
			{
				Spacing = 23,
				Children = { logo, lblActivation, lblHallID, accountID, lblUserID, userID, btnSubmit }
			};

			txtHallID1.TextChanged += Entry_TextChanged;
			txtHallID2.TextChanged += Entry_TextChanged;
			txtHallID3.TextChanged += Entry_TextChanged;
			txtHallID4.TextChanged += Entry_TextChanged;
			txtHallID5.TextChanged += Entry_TextChanged;
			txtHallID6.TextChanged += Entry_TextChanged;
			txtUserID1.TextChanged += Entry_TextChanged;
			txtUserID2.TextChanged += Entry_TextChanged;

			InitializeComponent();

			Console.Write(SettingsInterface);
			string sFireHallAndUserID = SettingsInterface.GetAccountInfoFromUserDefaults();

			// prepopulate fields if the device already has firehallid and userid set
			if (sFireHallAndUserID != ":")
			{
				Array aFireHallAndUserID = sFireHallAndUserID.Split(":".ToCharArray());
				var sFireHallID = aFireHallAndUserID.GetValue(0) as string;
				var sUserID = aFireHallAndUserID.GetValue(1) as string;

				txtHallID1.Text = sFireHallID.Substring(0, 1);
				txtHallID2.Text = sFireHallID.Substring(1, 1);
				txtHallID3.Text = sFireHallID.Substring(2, 1);
				txtHallID4.Text = sFireHallID.Substring(3, 1);
				txtHallID5.Text = sFireHallID.Substring(4, 1);
				txtHallID6.Text = sFireHallID.Substring(5, 1);

				txtUserID1.Text = sUserID.Substring(0, 1);
				txtUserID2.Text = sUserID.Substring(1, 1);

				btnSubmit.BackgroundColor = Color.Gray;
			}
		}

		private void SubmitButtonPressed(object sender, EventArgs e)
		{
            if (txtHallID1.Text != "" && txtHallID2.Text != "" && txtHallID3.Text != "" && txtHallID4.Text != "" && txtHallID5.Text != ""
               && txtHallID6.Text != "" && txtUserID1.Text != "" && txtUserID2.Text != "")
            {
                string sFireHallID = txtHallID1.Text + txtHallID2.Text + txtHallID3.Text + txtHallID4.Text + txtHallID5.Text + txtHallID6.Text;
                string sUserID = txtUserID1.Text + txtUserID2.Text;
                string result = SettingsInterface.SubmitAccountInfo(sFireHallID, sUserID);
                if (result == "OK" || result == "Admin") {
                    parentPage.SwitchToMainTab();
                    LocationInterface.AskForLocationPermissions();
                }
			}
		}

		void Entry_TextChanged(object sender, TextChangedEventArgs e)
		{
			var thisEntry = sender as Entry;
			string text = thisEntry.Text;      //Get Current Text
			Console.WriteLine(text);
			if (text.Length > 1)
			{
				text = text.Remove(0, 1);  // Remove First character
				thisEntry.Text = text.ToUpper(); // Capitalize
			}
			else {
				thisEntry.Text = text.ToUpper(); // Capitalize
			}

			if (sender == txtHallID1 && text != string.Empty)
			{
				txtHallID2.Focus();
			}
			if (sender == txtHallID2 && text != string.Empty)
			{
				txtHallID3.Focus();
			}
			if (sender == txtHallID3 && text != string.Empty)
			{
				txtHallID4.Focus();
			}
			if (sender == txtHallID4 && text != string.Empty)
			{
				txtHallID5.Focus();
			}
			if (sender == txtHallID5 && text != string.Empty)
			{
				txtHallID6.Focus();
			}
			if (sender == txtHallID6 && text != string.Empty)
			{
				txtUserID1.Focus();
			}
			if (sender == txtUserID1 && text != string.Empty)
			{
				txtUserID2.Focus();
			}
			if (sender == txtUserID2 && text != string.Empty)
			{
				txtUserID2.Unfocus();
			}

			if (txtHallID1.Text != "" && txtHallID2.Text != "" && txtHallID3.Text != "" && txtHallID4.Text != "" && txtHallID5.Text != ""
			   && txtHallID6.Text != "" && txtUserID1.Text != "" && txtUserID2.Text != "")
			{
				btnSubmit.BackgroundColor = Color.Orange;
			}
		}
	}
}
