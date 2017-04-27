using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class MainPage : TabbedPage
	{

		Page mainTab;
		RespondingTab responderTab;
		//Page responderTab;
		//Page availableTab;
		Page settingsTab;

		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
		public SettingsTabInterface SettingsInterface = DependencyService.Get<SettingsTabInterface>();

		public MainPage()
		{
			InitializeComponent();

			mainTab = new MainTab(this);
			mainTab.Title = "Main";

			responderTab = new RespondingTab();
			responderTab.Title = "Responding";

			settingsTab = new SettingsTab(this);
			settingsTab.Title = "Settings";

			Children.Add(mainTab);
			Children.Add(responderTab);
			Children.Add(settingsTab);

			responderTab.GetResponders();
			var seconds = TimeSpan.FromSeconds(10);
			Device.StartTimer(seconds, () =>
			{
				responderTab.GetResponders();

				return true;
			});

			// check if user has firehall id and user id stored on their device already.  If they do, go to main tab, if they don't, go to settings tab.
			string sFireHallAndUserID = SettingsInterface.GetAccountInfoFromUserDefaults();
			if (sFireHallAndUserID != ":")
			{
				CurrentPage = mainTab;
			}
			else
			{
				CurrentPage = settingsTab;
			}
			LocationInterface.RegisterForPushNotifications();
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
