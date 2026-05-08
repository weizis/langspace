using System.Collections.Generic;
using System.Linq;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.Views;

namespace LangSpace.ViewModel
{
    public class NotesPageViewModel : BaseViewModel
    {
        private readonly NoteService _noteService;
        private readonly int _currentUserId;

        private List<Note> _displayedNotes = new();
        private List<string> _filterTypes = new();
        private string _selectedFilter = "Все записи";

        public List<Note> DisplayedNotes
        {
            get { return _displayedNotes; }
            private set
            {
                _displayedNotes = value;
                OnPropertyChanged("DisplayedNotes");
            }
        }

        public List<string> FilterTypes
        {
            get { return _filterTypes; }
            private set
            {
                _filterTypes = value;
                OnPropertyChanged("FilterTypes");
            }
        }

        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter == value)
                {
                    return;
                }

                _selectedFilter = value;
                OnPropertyChanged("SelectedFilter");
                ApplyFilter();
            }
        }

        public NotesPageViewModel()
        {
            _noteService = new NoteService();
            _currentUserId = LoginWindow.currentUser?.IdUser ?? 0;
        }

        public void LoadData()
        {
            LoadFilterTypes();
            ApplyFilter();
        }

        public void Reload()
        {
            ApplyFilter();
        }

        private void LoadFilterTypes()
        {
            using var context = new LangSpaceContext();

            var resourceTypes = context.Resources
                .Select(r => r.ResourceType)
                .Distinct()
                .ToList();

            var filters = new List<string> { "Все записи" };

            if (!resourceTypes.Contains("Текстовая заметка"))
            {
                filters.Add("Текстовая заметка");
            }

            filters.AddRange(resourceTypes);

            FilterTypes = filters.Distinct().ToList();

            if (!FilterTypes.Contains(SelectedFilter))
            {
                SelectedFilter = "Все записи";
            }
        }

        private void ApplyFilter()
        {
            if (_currentUserId <= 0)
            {
                DisplayedNotes = new List<Note>();
                return;
            }

            var selected = string.IsNullOrWhiteSpace(SelectedFilter) ? "Все записи" : SelectedFilter;
            DisplayedNotes = _noteService.GetNotesByFilter(_currentUserId, selected);
        }
    }
}