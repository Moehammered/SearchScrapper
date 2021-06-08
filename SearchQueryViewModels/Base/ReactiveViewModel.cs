using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SearchQueryViewModels.Base
{
    public abstract class ReactiveViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string property = "")
        {
            if (!string.IsNullOrWhiteSpace(property))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
