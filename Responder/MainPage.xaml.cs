using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			InitializeComponent();

			var mainTab = new NavigationPage(new MainTab());
			mainTab.Title = "Main";

			var responderTab = new NavigationPage(new RespondingTab());
			responderTab.Title = "Respond";

			var availableTab = new NavigationPage(new AvailabilityTab());
			availableTab.Title = "Available";

			var settingsTab = new NavigationPage(new SettingsTab());
			settingsTab.Title = "Settings";

			NavigationPage.SetHasNavigationBar(this, false);
			NavigationPage.SetHasNavigationBar(mainTab, false);
			NavigationPage.SetHasNavigationBar(responderTab, false);
			NavigationPage.SetHasNavigationBar(availableTab, false);
			NavigationPage.SetHasNavigationBar(settingsTab, false);

			Children.Add(mainTab);
			Children.Add(responderTab);
			Children.Add(availableTab);
			Children.Add(settingsTab);
		}
	}
}
