using Xamarin.Forms;
//using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading;

namespace Responder
{
	public partial class App : Application
	{
        public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();
        //public MainPage mainPage;
        //class TimerState
        //{
        //    public int counter = 0;
        //    public Timer tmr;
        //}

		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
            //mainPage = (MainPage)MainPage;
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
            // Handle when your app sleeps
            // dispose of preexisting timer if there is one (there shouldnt be)
            //if (mainPage.mainTab.s.tmr != null)
            //{
            //    mainPage.mainTab.s.tmr.Dispose();
            //    mainPage.mainTab.s.tmr = null;
            //}

            // start background location updates
            DependencyService.Get<GetLocationInterface>().StartMonitoringLocationInBackground();
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
			if (MainTab.responding)
			{
				DependencyService.Get<GetLocationInterface>().StopMonitoringLocationChanges();

                // instantiate new timer
                //TimerCallback timerDelegate = new TimerCallback(CheckStatus);
                //Timer timer = new Timer(timerDelegate, mainPage.mainTab.s, 10000, 10000);
                //mainPage.mainTab.s.tmr = timer;
			}
		}

        //public void CheckStatus(Object state)
        //{
        //    TimerState timerState = (TimerState)state;
        //    if (MainTab.responding)
        //    {
        //        string result = LocationInterface.GetLocation();

        //        // Returning true means you want to repeat this timer, false stops it.
        //        if (result.Contains("AtHall"))
        //        {
        //            MainTab.responding = false;
        //            timerState.tmr.Dispose();
        //            timerState.tmr = null;
        //        }
        //    }
        //}
	}
}
