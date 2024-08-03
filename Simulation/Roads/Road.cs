using Simulation.Core;

namespace Simulation.Roads
{
	/// <summary>
	/// Реализация дороги на основе <see cref="SimulationEntityBase"/>
	/// </summary>
	public class Road : SimulationEntityBase, IRoad
	{
		/// <summary>
		/// Объект для синхронизации размера очереди.
		/// </summary>
		private readonly object _QueueSizeSyncRoot = new object();
		private int _QueueSize;
		public int QueueSize
		{
			get
			{
				lock (_QueueSizeSyncRoot)
					return _QueueSize;
			}
		}

		public event RoadQueueSizeChanged? QueueSizeChanged;

		/// <summary>
		/// Минимальное время ожидания следующей машины.
		/// </summary>
		private readonly int _MinIntervalMs;
		/// <summary>
		/// Максимальное время ожидания следующей машины.
		/// </summary>
		private readonly int _MaxIntervalMs;

		/// <summary>
		/// Создать дорогу, следующая машина на которой появляется через случайное время в интервале [<paramref name="minIntervalMs"/>; <paramref name="maxIntervalMs"/>] после предыдущей.
		/// </summary>
		/// <param name="minIntervalMs">Нижняя граница интервала</param>
		/// <param name="maxIntervalMs">Верхняя граница интервала</param>
		/// <exception cref="ArgumentOutOfRangeException">Бросается, если интервал некорректный или одна из границ меньше 1 мс.</exception>
		public Road(int minIntervalMs, int maxIntervalMs)
		{
			if (minIntervalMs <= 0)
				throw new ArgumentOutOfRangeException(nameof(minIntervalMs), "Interval's lower boundary should be at least 1 ms.");
			else if (maxIntervalMs < minIntervalMs)
				throw new ArgumentOutOfRangeException(nameof(maxIntervalMs), "Interval's upper boundary should be greater than or equal to lower boundary.");

			_MinIntervalMs = minIntervalMs;
			_MaxIntervalMs = maxIntervalMs + 1;
			_QueueSize = 0;
		}

		protected override void SimulationStep()
		{
			Thread.Sleep(Random.Shared.Next(_MinIntervalMs, _MaxIntervalMs));
			
			lock (_QueueSizeSyncRoot)
				_QueueSize++;

			QueueSizeChanged?.Invoke(this);
			//Не даём потоку уснуть.
			TryAwakeSimulation();
		}

		public void PopQueue()
		{
			lock (_QueueSizeSyncRoot)
				_QueueSize--;

			QueueSizeChanged?.Invoke(this);
		}
	}
}
