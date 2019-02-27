using GeoClient.Models;
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

        public IncidentsViewModel(Command loadItemsCommand)
        {
            Title = "Aufträge / Einsätze";
            EmptyListMessage = "Daten wurden noch nicht geladen.";
            IsListEmpty = true;
            Incidents = new ObservableCollection<IncidentItem>();
            Incidents.CollectionChanged += (sender, args) => OnCollectionChanged();
            LoadItemsCommand = loadItemsCommand;
        }

        private void OnCollectionChanged()
        {
            var isListEmpty = Incidents.Count == 0;
            IsListEmpty = isListEmpty;
        }
    }
}