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

			layout.Children.Add(new Label()
			{
				Text = "N/A",
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				WidthRequest = 50
			});

			var respondersSection = new TableSection("Responders");

			table.Root = new TableRoot() {
				new TableSection("My Info") {
					new ViewCell() {View = layout}
				},
				respondersSection
			};

			Content = table;
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

				layout.Children.Add(new Label()
				{
					Text = result.FullName,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.StartAndExpand
				});

				layout.Children.Add(new Label()
				{
					Text = result.DistanceFromHall,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.EndAndExpand,
					WidthRequest = 50,
					HorizontalTextAlignment = TextAlignment.End
				});

				layout.Children.Add(new Label()
				{
					Text = result.TimeToHall,
					TextColor = Color.Black,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.EndAndExpand,
					WidthRequest = 50
				});
				CellList.Add(new ViewCell() { View = layout });
			}

			return CellList;
		}

		public void GetResponders()
		{
			List<ResponderResult> results = DependencyService.Get<GetLocationInterface>().GetAllResponders();

			var table = new TableView();
			table.Intent = TableIntent.Settings;

			List<ViewCell> CellList = AddResponders(table, results);

			var layout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal
			};

			layout.Children.Add(new Label()
			{
				Text = results[0].FullName,
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.StartAndExpand
			});

			layout.Children.Add(new Label()
			{
				Text = results[0].DistanceFromHall,
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				WidthRequest = 50,
				HorizontalTextAlignment = TextAlignment.End
			});

			layout.Children.Add(new Label()
			{
				Text = results[0].TimeToHall,
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				WidthRequest = 50
			});

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

			Content = table;

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

				layout.Children.Add(new Label()
				{
					Text = results[0].FullName,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.StartAndExpand
				});

				layout.Children.Add(new Label()
				{
					Text = results[0].DistanceFromHall,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.EndAndExpand,
					WidthRequest = 50,
					HorizontalTextAlignment = TextAlignment.End
				});

				layout.Children.Add(new Label()
				{
					Text = results[0].TimeToHall,
					TextColor = Color.Black,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.EndAndExpand,
					WidthRequest = 50
				});

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

				Content = table;

				// Returning true means you want to repeat this timer, false stops it.
				return true;
			});
		}
	}
}
