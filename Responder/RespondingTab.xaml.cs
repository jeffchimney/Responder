using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Responder
{
	public partial class RespondingTab : ContentPage
	{
		public GetLocationInterface LocationInterface = DependencyService.Get<GetLocationInterface>();

		public RespondingTab()
		{
			InitializeComponent();

			Padding = new Thickness(25);

			Image logo = new Image()
			{
				Source = "firehalllogo.png",
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start
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
				Children = { logo, table }
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

				// set up layout of cell
				layout.Children.Add(new Label()
				{
					Text = result.FullName,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Start
				});

				layout.Children.Add(new Label()
				{
					Text = result.DistanceFromHall,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					WidthRequest = 50,
					HorizontalTextAlignment = TextAlignment.End
				});

				layout.Children.Add(new Label()
				{		
					Text = result.TimeToHall,
					TextColor = Color.FromHex("#f35e20"),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.End,
					WidthRequest = 75,
					HorizontalTextAlignment = TextAlignment.End
				});

				CellList.Add(new ViewCell() { View = layout });
			}

			return CellList;
		}

		public void ClearRespondersTable()
		{
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
			// get all responders (user info is first result in list)
			List<ResponderResult> results = LocationInterface.GetAllResponders();
			ResponderResult myResult;
			// assign first result and remove it from the list.
			if (results != null && results.Count > 0)
			{
				myResult = results[0];
				results.Remove(myResult);
			}
			else
			{
				myResult = new ResponderResult("N/A", "N/A");
			}

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

			// set up layout of 'My Info' cell
			layout.Children.Add(new Label()
			{
				Text = myResult.FullName,
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Start
			});

			layout.Children.Add(new Label()
			{
				Text = myResult.DistanceFromHall,
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = 50,
				HorizontalTextAlignment = TextAlignment.End
			});

			string sTimeToHall = "";
			if (myResult.TimeToHall == string.Empty)
			{
				sTimeToHall = "N/A";
			}
			else
			{
				sTimeToHall = myResult.TimeToHall;
			}

			layout.Children.Add(new Label()
			{
				Text = sTimeToHall,
				TextColor = Color.FromHex("#f35e20"),
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.End,
				WidthRequest = 75,
				HorizontalTextAlignment = TextAlignment.End
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

			var VerticalLayout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical
			};

			VerticalLayout.Children.Add(logo);
			VerticalLayout.Children.Add(table);

			Content = VerticalLayout;
			Console.WriteLine("Getting all responders");
		}
	}
}
