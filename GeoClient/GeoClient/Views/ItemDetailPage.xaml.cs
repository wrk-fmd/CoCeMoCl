using GeoClient.Models;
using GeoClient.Models.Action;
using GeoClient.ViewModels;
using GeoClient.Views.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        private const string pleaseTryAgain = " Bitte lade die Einsätze neu und probiere es erneut.";
        private readonly ItemDetailViewModel _viewModel;
        private HttpClient _httpClient;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

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
                Children = { label, statusIcon },
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

            var uriList = GeoPointUtil.CreateUrisForGeoPoint(item.Location, "GeoClient: Berufungsort");

            if (uriList != null)
            {
                await OpenFirstSupportedUri(uriList);
            }
            else
            {
                await ShowGeoUriNotAvailableError();
            }
        }

        private async void OpenDestination_Clicked(object sender, EventArgs e)
        {
            var item = _viewModel.IncidentItem;

            var uriList = GeoPointUtil.CreateUrisForGeoPoint(item.Destination, "GeoClient: Zielort");

            if (uriList != null)
            {
                await OpenFirstSupportedUri(uriList);
            }
            else
            {
                await ShowGeoUriNotAvailableError();
            }
        }

        private async void SetNextState_Clicked(object sender, EventArgs args)
        {
            var action = _viewModel.IncidentItem?.NextStateAction;
            if (action != null)
            {
                await ExecuteSetNextState(action);
            }
            else
            {
                await ShowNextStateNotPossibleError();
            }
        }

        private async Task ExecuteSetNextState(NextStateActionOfIncident action)
        {

            string confirmationMessage;

            if (_viewModel.IncidentItem?.Type == GeoIncidentType.Relocation && action.PlannedState == OneTimeActionTaskState.AAO)
            {
                confirmationMessage = "Soll der Status auf 'Standort halten am Zielort' gesetzt werden?";
            }
            else if (action.PlannedState == OneTimeActionTaskState.DETACHED)
            {
                confirmationMessage = "Soll der Einsatz beendet werden?";
            }
            else
            {
                confirmationMessage = $"Soll der Status der Einheit auf '{action.PlannedState}' geändert werden?";
            }

            bool confirmed = await DisplayAlert("Status setzen", confirmationMessage, "Ja", "Nein");
            if (confirmed)
            {
                await Navigation.PushAsync(new StatusSendingModalPage());
                await _httpClient.PostAsync(action.Url, new StringContent("")).ContinueWith(actionResponse =>
                {
                    var resultCode = NextStateActionResultCode.OPERATION_FAILED;
                    try
                    {
                        resultCode = GetResultCodeOfOperation(actionResponse);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to parse one-time-action response from server!");
                        Console.WriteLine(e.ToString());
                    }

                    ReportFinishedStateChange(resultCode, action.PlannedState);
                });
            }
        }

        private void ReportFinishedStateChange(NextStateActionResultCode resultCode, OneTimeActionTaskState plannedState)
        {
            IReadOnlyList<Page> stack = Navigation.NavigationStack;
            if (stack.Count > 0 && stack[stack.Count - 1].GetType() == typeof(StatusSendingModalPage))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ShowStateChangeFeedback(resultCode, plannedState);
                });
            }
        }

        private async Task ShowStateChangeFeedback(NextStateActionResultCode resultCode, OneTimeActionTaskState plannedState)
        {
            await Navigation.PopToRootAsync();
            string resultMessage;
            switch (resultCode)
            {
                case NextStateActionResultCode.SUCCESS:
                    resultMessage = CreateSuccessMessageForPlannedState(plannedState);
                    break;
                case NextStateActionResultCode.ACTION_ID_OUTDATED:
                    resultMessage = "Der Status konnte nicht gesetzt werden. Der Status wurde bereits von der Leitstelle verändert." + pleaseTryAgain;
                    break;
                default:
                    resultMessage = $"Der Status konnte nicht gesetzt werden. Der interne Fehlercode ist: {resultCode}." + pleaseTryAgain;
                    break;
            }

            await DisplayAlert("Statusänderung", resultMessage, "OK");
        }

        private static string CreateSuccessMessageForPlannedState(OneTimeActionTaskState plannedState)
        {
            return plannedState == OneTimeActionTaskState.DETACHED
                ? "Der Status wurde erfolgreich auf 'Einsatz beendet' gesetzt."
                : $"Der Status wurde erfolgreich auf '{plannedState}' gesetzt.";
        }

        private static NextStateActionResultCode GetResultCodeOfOperation(Task<HttpResponseMessage> actionResponse)
        {
            var responseString = actionResponse.Result.Content.ReadAsStringAsync().Result;
            JObject responseJson = JObject.Parse(responseString);
            string resultCodeString = responseJson.Value<string>("resultCode");

            return (NextStateActionResultCode)Enum.Parse(typeof(NextStateActionResultCode), resultCodeString);
        }

        private async Task OpenFirstSupportedUri(List<Uri> uriList)
        {
            bool uriOpened = false;
            foreach (Uri uri in uriList)
            {
                var uriSupported = await Launcher.CanOpenAsync(uri);
                if (uriSupported)
                {
                    await Launcher.OpenAsync(uri);
                    uriOpened = true;
                    break;
                }
            }

            if (!uriOpened)
            {
                await ShowNoAppForGeoUriError();
            }
        }

        private async Task ShowGeoUriNotAvailableError()
        {
            await DisplayAlert(
                "Adresse nicht verortet",
                "Die Adresse konnte leider nicht automatisch verortet werden.",
                "OK");
        }

        private async Task ShowNoAppForGeoUriError()
        {
            await DisplayAlert(
                "Keine passende App gefunden",
                "Es konnte keine passende App für den Link gefunden werden. Bitte installiere ein Kartenprogramm wie z.B. Google Maps.",
                "OK");
        }

        private async Task ShowNextStateNotPossibleError()
        {
            await DisplayAlert(
                "Statusänderung nicht möglich",
                "Der Status kann aktuell nicht über die App geändert werden. Möglicherweise fehlt ein Zielort oder eine Bettenbuchung um den nächsten Status auswählen zu können.",
                "OK");
        }
    }
}