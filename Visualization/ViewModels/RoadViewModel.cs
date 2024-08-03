using Simulation.Roads;
using System;

namespace Visualization.ViewModels
{
    class RoadViewModel : ViewModelBase
    {
        private readonly IRoad _Road;

        public RoadViewModel(IContext context, IRoad road) : base(context)
        {
            _Road = road ?? throw new ArgumentNullException(nameof(road));
			road.QueueSizeChanged += Road_QueueSizeChanged;
        }

        private int _QueueSize;
        public int QueueSize
        {
            get
            {
                lock (SyncRoot)
                    return _QueueSize;
            }
            protected set
            {
                bool hasChanged = false;
                lock (SyncRoot)
                {
                    hasChanged = _QueueSize != value;
                    _QueueSize = value;
                }
                if (hasChanged)
                    InvokePropertyChanged();
            }
        }


		private void Road_QueueSizeChanged(IRoad road)
		{
			if (road != _Road)
                return;

            QueueSize = road.QueueSize;
		}
	}
}
