using Simulation.Core;
using Simulation.Roads;

namespace Simulation.Cameras
{
	/// <summary>
	/// Реализация камеры светофора на основе <see cref="SimulationEntityBase"/>
	/// </summary>
	public class TrafficLightCamera : SimulationEntityBase, ITrafficLightCamera
	{
		private int _LastQueueSize;
		public int QueueSize
		{
			get
			{
				lock (_QueueSizeLock)
					return _LastQueueSize;
			}
		}

		public event TrafficLightCameraQueueSizeChanged? QueueSizeChanged;

		/// <summary>
		/// Дорога, за которой наблюдаем
		/// </summary>
		private readonly IRoad _Road;
		/// <summary>
		/// Частота наблюдения в мс.
		/// </summary>
		private readonly int _PollingInterval;

		/// <summary>
		/// Объект для синхронизации доступа к размеру очереди.
		/// </summary>
		private readonly object _QueueSizeLock = new object();

		/// <summary>
		/// Создать камеру, наблюдающую за указанной дорогой.
		/// </summary>
		/// <param name="road">Дорога для наблюдения.</param>
		/// <param name="pollingIntervalMs">Частота обновления размера очереди дороги.</param>
		/// <exception cref="ArgumentOutOfRangeException">Бросается, если <paramref name="pollingIntervalMs"/> меньше 1 мс.</exception>
		/// <exception cref="ArgumentNullException">Бросается, если <paramref name="road"/> имеет значение <c>null</c></exception>
		public TrafficLightCamera(IRoad road, int pollingIntervalMs)
		{
			if (pollingIntervalMs <= 0)
				throw new ArgumentOutOfRangeException(nameof(pollingIntervalMs), "Polling frequency cannot be less than 1 ms.");

			_PollingInterval = pollingIntervalMs;
			_Road = road ?? throw new ArgumentNullException(nameof(road));
			_LastQueueSize = 0;
		}

		protected override void SimulationStep()
		{
			//Поллим состояние очереди дороги каждые _PollingInterval мс.
			Thread.Sleep(_PollingInterval);
			lock (_QueueSizeLock)
			{
				int oldSize = _LastQueueSize;
				_LastQueueSize = _Road.QueueSize;
				if (oldSize != _LastQueueSize)
					QueueSizeChanged?.Invoke(this);
			}
			//Не даём потоку уснуть.
			TryAwakeSimulation();
		}
	}
}
