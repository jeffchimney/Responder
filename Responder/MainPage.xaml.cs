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

			mainTab = new NavigationPage(new MainTab());
			mainTab.Title = "Main";

			//responderTab = new NavigationPage(new RespondingTab());
			//responderTab.Title = "Respond";

			//availableTab = new NavigationPage(new AvailabilityTab());
			//availableTab.Title = "Available";

			settingsTab = new NavigationPage(new SettingsTab());
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

			CurrentPage = settingsTab;
		}

		public void SwitchToMainTab()
		{
			mainTab = new NavigationPage(new MainTab());
			mainTab.Title = "Main";
			CurrentPage = mainTab;
		}
		public void SwitchToSettingsTab()
		{
			CurrentPage = settingsTab;
		}
	}
}
