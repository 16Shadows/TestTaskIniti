using Simulation.Roads;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Visualization.ViewModels
{
    class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly IContext Context;
        protected readonly object SyncRoot = new object();

        public ViewModelBase(IContext context)
        {
			Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void InvokePropertyChanged([CallerMemberName] string name = "")
        {
            Context.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }
    }
}
