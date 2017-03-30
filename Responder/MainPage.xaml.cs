using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class MainPage : TabbedPage
	{

		Page mainTab;
		RespondingTab responderTab;
		//Page availableTab;
		Page settingsTab;

		public MainPage()
		{
			InitializeComponent();

			mainTab = new NavigationPage(new MainTab(this));
			mainTab.Title = "Main";

			responderTab = new RespondingTab();
			responderTab.Title = "Responding";

			//availableTab = new NavigationPage(new AvailabilityTab());
			//availableTab.Title = "Available";

			settingsTab = new NavigationPage(new SettingsTab(this));
			settingsTab.Title = "Settings";

			NavigationPage.SetHasNavigationBar(this, false);
			NavigationPage.SetHasNavigationBar(mainTab, false);
			NavigationPage.SetHasNavigationBar(responderTab, false);
			//NavigationPage.SetHasNavigationBar(availableTab, false);
			NavigationPage.SetHasNavigationBar(settingsTab, false);

			Children.Add(mainTab);
			Children.Add(responderTab);
			//Children.Add(availableTab);
			Children.Add(settingsTab);

			this.CurrentPageChanged += (object sender, EventArgs e) =>
			{
				var i = this.Children.IndexOf(this.CurrentPage);

				if (i == 1) // Responding page selected
				{ // start getting others responding on the responding tab
					var result = DependencyService.Get<GetLocationInterface>().GetAllResponders();
					responderTab.GetResponders();
					var seconds = TimeSpan.FromSeconds(30);
					Device.StartTimer(seconds, () =>
					{
						responderTab.GetResponders();

						return true;
					});
				}
				System.Diagnostics.Debug.WriteLine("Page No:" + i);
			};

			// check if user has firehall id and user id stored on their device already.  If they do, go to main tab, if they don't, go to settings tab.
			string sFireHallAndUserID = DependencyService.Get<SettingsTabInterface>().GetAccountInfoFromUserDefaults();
			if (sFireHallAndUserID != ":")
			{
				CurrentPage = mainTab;
			}
			else
			{
				CurrentPage = settingsTab;
			}
			DependencyService.Get<GetLocationInterface>().RegisterForPushNotifications();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}

		public void SwitchToMainTab()
		{
			CurrentPage = mainTab;
		}
		public void SwitchToSettingsTab()
		{
			CurrentPage = settingsTab;
		}
	}
}
