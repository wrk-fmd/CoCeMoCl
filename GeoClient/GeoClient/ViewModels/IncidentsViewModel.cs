using GeoClient.Models;
using GeoClient.Services.Boundary;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GeoClient.ViewModels
{
    public class IncidentsViewModel : BaseViewModel
    {
        public ObservableCollection<IncidentItem> Incidents { get; }
        public Command LoadItemsCommand { get; }

        private bool _isBusy;
        private string _emptyListMessage;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string EmptyListMessage
        {
            get => _emptyListMessage;
            set => SetProperty(ref _emptyListMessage, value);
        }

        private bool _isListEmpty;

        public bool IsListEmpty
        {
            get => _isListEmpty;
            set => SetProperty(ref _isListEmpty, value);
        }

        private readonly RestService _restService;

        public IncidentsViewModel()
        {
            Title = "Aufträge / Einsätze";
            EmptyListMessage = "Daten wurden noch nicht geladen.";
            IsListEmpty = true;
            Incidents = new ObservableCollection<IncidentItem>();
            Incidents.CollectionChanged += (sender, args) => OnCollectionChanged();
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);

            _restService = RestService.Instance;
        }

        private void OnCollectionChanged()
        {
            var isListEmpty = Incidents.Count == 0;
            IsListEmpty = isListEmpty;
        }

        private void ExecuteLoadItemsCommand()
        {
            if (IsBusy)
            {
                Console.WriteLine("Update of incidents is still busy.");
                return;
            }

            IsBusy = true;

            _restService.GetScope();
        }
    }
}