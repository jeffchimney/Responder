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

		Label lblAccountID = new Label
		{
			Text = "Account ID",
			FontSize = 15,
			HorizontalTextAlignment = TextAlignment.Start
		};

		Entry txtAccountID = new Entry
		{
			Placeholder = "Account ID"
		};

		Label lblUserID = new Label
		{
			Text = "User ID",
			FontSize = 15,
			HorizontalTextAlignment = TextAlignment.Start
		};

		Entry txtUserID = new Entry
		{
			Placeholder = "Account ID"
		};

		Label lblPassword = new Label
		{
			Text = "Password",
			FontSize = 15,
			HorizontalTextAlignment = TextAlignment.Start
		};

		Entry txtPassword = new Entry
		{
			Placeholder = "Password"
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
				Children = { lblAccountID, txtAccountID }
			};

			StackLayout userID = new StackLayout
			{
				Spacing = 5,
				Children = { lblUserID, txtUserID }
			};

			StackLayout password = new StackLayout
			{
				Spacing = 5,
				Children = { lblPassword, txtPassword }
			};

			Content = new StackLayout
			{
				Spacing = 23,
				Children = { lblTitle, lblActivation, accountID, userID, password, btnSubmit }
			};

			InitializeComponent();
		}

		private void SubmitButtonPressed(object sender, EventArgs e)
		{
			if (txtAccountID.Text != "" && txtUserID.Text != "" && txtPassword.Text != "")
			{
				// call iOS/Android specific code to respond to the button click
				DependencyService.Get<SettingsTabInterface>().SubmitAccountInfo();
			}
		}
	}
}
