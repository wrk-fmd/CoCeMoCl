using GeoClient.Models;
using GeoClient.ViewModels;
using GeoClient.Views.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        private readonly ItemDetailViewModel _viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;

            InitializeUnitList();
        }

        private void InitializeUnitList()
        {
            UnitList.Children.Add(CreateHorizontalLine());
            _viewModel.IncidentItem.Units.ForEach(unit =>
            {
                UnitList.Children.Add(CreateAssignedUnitLine(unit));
                UnitList.Children.Add(CreateHorizontalLine());
            });
        }

        private static BoxView CreateHorizontalLine()
        {
            return new BoxView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 1,
                BackgroundColor = Color.DarkGray
            };
        }

        private static StackLayout CreateAssignedUnitLine(UnitOfIncident unitToBind)
        {
            var label = CreateUnitLabel();
            var statusIcon = CreateStatusIconImage();

            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {label, statusIcon},
                BindingContext = unitToBind
            };
        }

        private static Image CreateStatusIconImage()
        {
            var statusIcon = new Image
            {
                Scale = 0.7,
                HorizontalOptions = LayoutOptions.End
            };
            statusIcon.SetBinding(Image.SourceProperty, "TaskStateIcon");
            return statusIcon;
        }

        private static Label CreateUnitLabel()
        {
            var label = new Label
            {
                LineBreakMode = LineBreakMode.NoWrap,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            label.SetBinding(Label.TextProperty, "Name");
            return label;
        }

        private async void OpenLocation_Clicked(object sender, EventArgs e)
        {
            var item = _viewModel.IncidentItem;

            var geoUri = GeoPointUtil.CreateGeoUri(item.Location, "GeoClient: Berufungsort");

            if (geoUri != null)
            {
                await Launcher.OpenAsync(geoUri);
            }
            else
            {
                await ShowGeoUriNotAvailableError();
            }
        }

        private async void OpenDestination_Clicked(object sender, EventArgs e)
        {
            var item = _viewModel.IncidentItem;

            var geoUri = GeoPointUtil.CreateGeoUri(item.Destination, "GeoClient: Zielort");

            if (geoUri != null)
            {
                await Launcher.OpenAsync(geoUri);
            }
            else
            {
                await ShowGeoUriNotAvailableError();
            }
        }

        private async Task ShowGeoUriNotAvailableError()
        {
            await DisplayAlert(
                "Adresse nicht verortet",
                "Die Adresse konnte leider nicht automatisch verortet werden.",
                "OK");
        }
    }
}