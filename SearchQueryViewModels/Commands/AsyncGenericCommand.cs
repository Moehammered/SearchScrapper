using System;
using System.Threading.Tasks;
using System.Windows;

namespace SearchQueryViewModels.Commands
{
    public class AsyncGenericCommand<T> : GenericCommand<T>
    {
        public bool ShowErrorMessage { get; set; } = true;
        private Func<T, Task> AsyncCommand { get; set; }

        public AsyncGenericCommand(Func<T, Task> asyncCmd)
            : base((o) => { })
        {
            Command = ExecuteSafely;
            AsyncCommand = asyncCmd;
        }

        public AsyncGenericCommand(Func<T, Task> asyncCmd, Predicate<T> prd)
            : base((o) => { }, prd)
        {
            AsyncCommand = asyncCmd;
            Command = ExecuteSafely;
        }

        private async void ExecuteSafely(T obj)
        {
            try
            {
                await AsyncCommand(obj);
            }
            catch (Exception e)
            {
                if (ShowErrorMessage)
                    MessageBox.Show(e.Message);
            }
        }
    }
}
