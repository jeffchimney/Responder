using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class RespondingTab : ContentPage
	{
		public RespondingTab()
		{
			InitializeComponent();

			Padding = new Thickness(10);

			Image logo = new Image()
			{
				Source = "firehalllogo.png",
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center
			};

			var table = new TableView();
			table.Intent = TableIntent.Settings;

			var layout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal
			};

			layout.Children.Add(new Label()
			{
				Text = "Loading...",
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.StartAndExpand
			});

			layout.Children.Add(new Label()
			{
				Text = "N/A",
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				WidthRequest = 50,
				HorizontalTextAlignment = TextAlignment.End
			});

			var respondersSection = new TableSection("Responders");

			table.Root = new TableRoot() {
				new TableSection("My Info") {
					new ViewCell() {View = layout}
				},
				respondersSection
			};

			Content = new StackLayout
			{
				Spacing = 5,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { logo, table } //btnRespondingScene,btnOnScene,sideBySide,lblCoords, btnUnavailable
			};
		}

		public List<ViewCell> AddResponders(TableView table, List<ResponderResult> responderResults)
		{
			var CellList = new List<ViewCell>();
			foreach (ResponderResult result in responderResults)
			{
				var layout = new StackLayout()
				{
					Orientation = StackOrientation.Horizontal
				};

				CellList.Add(new ViewCell() { View = layout });
			}

			return CellList;
		}

		public void ClearRespondersTable()
		{
			//List<ResponderResult> results = DependencyService.Get<GetLocationInterface>().GetAllResponders();

			var table = new TableView();
			table.Intent = TableIntent.Settings;

			var layout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal
			};

			var respondersSection = new TableSection("Responders");

			table.Root = new TableRoot()
			{
				new TableSection("My Info") {
					new ViewCell() {View = layout}
				},
				respondersSection
			};

			Content = table;
		}

		public void GetResponders()
		{
			List<ResponderResult> results = DependencyService.Get<GetLocationInterface>().GetAllResponders();

			Image logo = new Image()
			{
				Source = "firehalllogo.png",
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center
			};

			var table = new TableView();
			table.Intent = TableIntent.Settings;

			List<ViewCell> CellList = AddResponders(table, results);

			var layout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal
			};

			var respondersSection = new TableSection("Responders");
			foreach (ViewCell cell in CellList)
			{
				respondersSection.Add(cell);
			}

			table.Root = new TableRoot() {
				new TableSection("My Info") {
					new ViewCell() {View = layout}
				},
				respondersSection
			};

			var VerticalLayout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical
			};

			VerticalLayout.Children.Add(logo);
			VerticalLayout.Children.Add(table);

			Content = VerticalLayout;

			// Run on a timer:

			var seconds = TimeSpan.FromSeconds(30);
			Device.StartTimer(seconds, () =>
			{

				//call your method to check for notifications here
				results = DependencyService.Get<GetLocationInterface>().GetAllResponders();

				table = new TableView();
				table.Intent = TableIntent.Settings;

				CellList = AddResponders(table, results);

				layout = new StackLayout()
				{
					Orientation = StackOrientation.Horizontal
				};

				respondersSection = new TableSection("Responders");
				foreach (ViewCell cell in CellList)
				{
					respondersSection.Add(cell);
				}

				table.Root = new TableRoot() {
					new TableSection("My Info") {
					new ViewCell() {View = layout}
					},
					respondersSection
				};

				Content = VerticalLayout;

				// Returning true means you want to repeat this timer, false stops it.
				return true;
			});
		}
	}
}
