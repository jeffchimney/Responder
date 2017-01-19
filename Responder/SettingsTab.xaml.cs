using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class SettingsTab : ContentPage
	{

		Label lblTitle = new Label
		{
			Text = "FH Responder",
			FontSize = 40,
			HorizontalTextAlignment = TextAlignment.Center
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
			WidthRequest = 50,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID2 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 50,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID3 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 50,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID4 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 50,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID5 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 50,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtHallID6 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 50,
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
			WidthRequest = 50,
			HeightRequest = 50,
			HorizontalOptions = LayoutOptions.Start,
			HorizontalTextAlignment = TextAlignment.Center,
			FontSize = 25
		};
		Entry txtUserID2 = new Entry
		{
			Placeholder = "A",
			WidthRequest = 50,
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
			TextColor = Color.Black
		};

		public SettingsTab()
		{
			//set up touch delegates
			btnSubmit.Clicked += SubmitButtonPressed;

			Padding = new Thickness(20);

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
				Children = { lblTitle, lblActivation, lblHallID, accountID, lblUserID, userID, btnSubmit }
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
		}

		private void SubmitButtonPressed(object sender, EventArgs e)
		{
			if (txtHallID1.Text != "" && txtHallID2.Text != "" && txtHallID3.Text != "" && txtHallID4.Text != "" && txtHallID5.Text != ""
			   && txtHallID6.Text != "" && txtUserID1.Text != "" && txtUserID2.Text != "")
			{
				string sFireHallID = txtHallID1.Text + txtHallID2.Text + txtHallID3.Text + txtHallID4.Text + txtHallID5.Text + txtHallID6.Text;
				string sUserID = txtUserID1.Text + txtUserID2.Text;
				// call iOS/Android specific code to respond to the button click
				DependencyService.Get<SettingsTabInterface>().SubmitAccountInfo(sFireHallID, sUserID);
			}
		}

		void Entry_TextChanged(object sender, TextChangedEventArgs e)
		{
			Entry thisEntry = sender as Entry;
			string text = thisEntry.Text;      //Get Current Text
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
				btnSubmit.BackgroundColor = Color.Green;
			}
		}
	}
}
