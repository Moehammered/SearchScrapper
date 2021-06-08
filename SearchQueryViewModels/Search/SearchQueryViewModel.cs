using SearchQueryViewModels.Base;

namespace SearchQueryViewModels.Search
{
    public class SearchQueryViewModel : ReactiveViewModel
    {
        private string searchTerm { get; set; } = string.Empty;
        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                searchTerm = value;
                RaisePropertyChanged();
            }
        }

        private string url { get; set; } = string.Empty;
        public string Url
        {
            get => url;
            set
            {
                url = value;
                RaisePropertyChanged();
            }
        }

        private string resultLimit { get; set; } = string.Empty;
        public string ResultLimit
        {
            get => resultLimit;
            set
            {
                resultLimit = value;
                RaisePropertyChanged();
            }
        }
    }
}
