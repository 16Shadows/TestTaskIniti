using System;
using System.Windows.Threading;

namespace Visualization.ViewModels
{
    /// <summary>
    /// Реализация <see cref="IContext"/> для WPF.
    /// </summary>
    class WpfContext : IContext
    {
        private readonly Dispatcher _Dispatcher;

        public WpfContext(Dispatcher dispatcher)
        {
            _Dispatcher = dispatcher;
        }

		public void Invoke(Action action)
		{
			_Dispatcher.InvokeAsync(action);
		}
	}
}
