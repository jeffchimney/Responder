using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class MainPage : TabbedPage
	{

		Page mainTab;
		//Page responderTab;
		//Page availableTab;
		Page settingsTab;

		public MainPage()
		{
			InitializeComponent();

			mainTab = new NavigationPage(new MainTab(this));
			mainTab.Title = "Main";

			//responderTab = new NavigationPage(new RespondingTab());
			//responderTab.Title = "Respond";

			//availableTab = new NavigationPage(new AvailabilityTab());
			//availableTab.Title = "Available";

			settingsTab = new NavigationPage(new SettingsTab(this));
			settingsTab.Title = "Settings";

			NavigationPage.SetHasNavigationBar(this, false);
			NavigationPage.SetHasNavigationBar(mainTab, false);
			//NavigationPage.SetHasNavigationBar(responderTab, false);
			//NavigationPage.SetHasNavigationBar(availableTab, false);
			NavigationPage.SetHasNavigationBar(settingsTab, false);

			Children.Add(mainTab);
			//Children.Add(responderTab);
			//Children.Add(availableTab);
			Children.Add(settingsTab);

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
