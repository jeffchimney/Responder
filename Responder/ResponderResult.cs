using System;
namespace Responder
{
	public class ResponderResult
	{
		public string FullName
		{
			get;
			set;
		}

		public string DistanceFromHall
		{
			get;
			set;
		}

		public string TimeToHall
		{
			get;
			set;
		}

		public ResponderResult(string sFullName, string sDistanceFromHall)
		{
			FullName = sFullName;
			DistanceFromHall = sDistanceFromHall;
		}

		public ResponderResult(string sFullName, string sDistanceFromHall, string sTimeToHall)
		{
			FullName = sFullName;
			DistanceFromHall = sDistanceFromHall;
			TimeToHall = sTimeToHall;
		}
	}
}
