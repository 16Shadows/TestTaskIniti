using Simulation.Cameras;
using Simulation.TrafficLights;
using System;

namespace Visualization.ViewModels
{
    enum TrafficLightColor
    {
        Red,
        Green
    }

    class TrafficLightViewModel : ViewModelBase
    {
        private readonly ITrafficLightCamera _Camera;
        private readonly ITrafficLight _TrafficLight;

        public TrafficLightViewModel(IContext context, ITrafficLightCamera camera, ITrafficLight trafficLight) : base(context)
        {
            _Camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _TrafficLight = trafficLight ?? throw new ArgumentNullException(nameof(trafficLight));
			_Camera.QueueSizeChanged += Camera_QueueSizeChanged;
			_TrafficLight.PassabilityChanged += TrafficLight_PassabilityChanged;
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

        private TrafficLightColor _TrafficLightColor;
        public TrafficLightColor TrafficLightColor
        {
            get
            {
                lock (SyncRoot)
                    return _TrafficLightColor;
            }
            protected set
            {
                bool hasChanged = false;
                lock (SyncRoot)
                {
                    hasChanged = _TrafficLightColor != value;
                    _TrafficLightColor = value;
                }
                if (hasChanged)
                    InvokePropertyChanged();
            }
        }

		private void Camera_QueueSizeChanged(ITrafficLightCamera camera)
		{
			if (camera != _Camera)
                return;

            QueueSize = camera.QueueSize;
		}

        private void TrafficLight_PassabilityChanged(ITrafficLight trafficLight)
		{
			if (_TrafficLight != trafficLight)
                return;

            TrafficLightColor = trafficLight.CanBePassed ? TrafficLightColor.Green : TrafficLightColor.Red;
		}
    }
}
