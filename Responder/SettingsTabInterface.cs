using System;
using Xamarin.Forms;
namespace Responder
{
	public interface SettingsTabInterface
	{
		string SubmitAccountInfo(string sFireHallID, string sUserID);
		string GetAccountInfoFromUserDefaults();
        bool IsAdmin();
	}
}
