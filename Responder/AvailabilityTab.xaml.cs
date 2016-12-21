using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class AvailabilityTab : ContentPage
	{

		Label lblTitle = new Label
		{
			Text = "FH Responder",
			FontSize = 40,
			HorizontalTextAlignment = TextAlignment.Center
		};

		public AvailabilityTab()
		{
			InitializeComponent();
		}
	}
}
