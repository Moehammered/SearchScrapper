using System;
using System.Windows.Input;

namespace SearchQueryViewModels.Commands
{
    public class GenericCommand<T> : ICommand
    {
        protected Action<T> Command { get; set; }
        protected Predicate<T> CommandPredicate { get; set; }

        private event EventHandler PredicateChanged;
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                PredicateChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                PredicateChanged -= value;
            }
        }

        public GenericCommand(Action<T> cmd)
            : this(cmd, (o) => true)
        {
        }

        public GenericCommand(Action<T> cmd, Predicate<T> prd)
        {
            Command = cmd;
            CommandPredicate = prd;
        }

        public bool CanExecute(T param)
            => CommandPredicate?.Invoke(param) ?? false;

        public void Execute(T param)
            => Command(param);

        public void OnCanExecuteChanged()
            => PredicateChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                return false;

            var converted = Convert.ChangeType(parameter, typeof(T));
            if (converted != null && converted is T casted)
                return CanExecute(casted);
            else
                return false;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                var converted = Convert.ChangeType(parameter, typeof(T));
                if (converted != null && converted is T casted)
                    Execute(casted);
            }
        }
    }
}
